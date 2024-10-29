using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LockedPlace : MonoBehaviour
{
    [SerializeField] private GameObject objToEnable;
    [SerializeField] private GameObject objToDisable;
    [SerializeField] private TMP_Text goldRemaining;
    [SerializeField] private Transform lerpPos;
    [SerializeField] private ParticleSystem UnlockVFX;

    private StringBuilder goldRemainingSb;

    private float lerpDelay = 0.03f;
    private float lerpSpeed = 50.0f;
    private bool isLerping = false;

    [SerializeField] private int goldToUnlock = 30;

    [SerializeField] private bool isPayable = false;

    private UnlockPlaceGold goldPaid;
    private int GoldPaid
    {
        get
        {
            if (goldPaid == null)
                goldPaid = UnlockPlaceGold.GetInstance(this);
            return goldPaid.GoldPaid;
        }
        set
        {
            if (goldPaid == null)
                goldPaid = UnlockPlaceGold.GetInstance(this);
            goldPaid.SetGold(value);
            OnGoldPaid();
        }
    }

    private UnityEvent OnTutorialClear = new UnityEvent();
    private void Awake()
    {
        TutorialArrowController.Instance.AddCondition(OnTutorialClear, 4);
    }
    private void Start()
    {
        OnGoldPaid();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            isPayable = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player)) { isPayable = false; }
    }

    private void OnTriggerStay(Collider other)
    {
        if (CanPay())
            StartCoroutine(CorPayMoney());
    }


    private IEnumerator CorPayMoney()
    {
        isLerping = true;
        var money = MoneyPoolManager.Instance.GetMoney();
        money.transform.position = lerpPos.transform.position;
        Vector3 destPos = lerpPos.position + Vector3.up * 5;

        while ((money.transform.position - destPos).sqrMagnitude > 0.1f)
        {
            money.transform.position = Vector3.Slerp(money.transform.position, destPos, Time.deltaTime * lerpSpeed);
            yield return null;
        }
        money.transform.position = destPos;
        MoneyPoolManager.Instance.ReturnMoney(money);
        MoneyModel.Instance.MinusGold(1);

        GoldPaid = GoldPaid + 1;
        yield return new WaitForSeconds(lerpDelay);
        isLerping = false;
    }

    private void UnlockObject()
    {
        if (TutorialArrowController.Instance.CurrentTutorialLevel == 5)
            OnTutorialClear.Invoke();
        objToEnable.SetActive(true);
        objToDisable.SetActive(false);
        UnlockVFX.Play();
    }

    private void SetGoldRemainingText(int goldAmount)
    {
        if (goldRemainingSb == null)
            goldRemainingSb = new StringBuilder();
        goldRemainingSb.Clear();
        goldRemainingSb.Append(goldAmount);

        goldRemaining.text = goldRemainingSb.ToString();
    }
    private void OnGoldPaid()
    {
        int remaining = goldToUnlock - GoldPaid;
        SetGoldRemainingText(remaining);
        IsUnlockObject();
    }
    private bool CanPay()
    {
        bool can = MoneyModel.Instance.GoldCount > 0 && GoldPaid < goldToUnlock && isPayable && !isLerping;
        return can;
    }
    private void IsUnlockObject()
    {
        if (GoldPaid >= goldToUnlock)
            UnlockObject();
    }
}
