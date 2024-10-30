using UnityEngine;

public class Table : MonoBehaviour
{
    [SerializeField] private GameObject Trash;

    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private Transform VFXPos;

    private ParticleSystem VFX_Clean;

    

    private bool isAvailable = false;
    public bool IsAvailable
    {
        get { return isAvailable; }
        set
        {
            if (isAvailable != value)
                isAvailable = value;
        }
    }
    [SerializeField] private Transform chairPos;
    public Transform ChairPos { get { return chairPos; } }

    private AudioSource audioSource;
    private AudioClip SFX_Trash;

    private void OnEnable()
    {
        IsAvailable = true;
        TableManager.Instance.AddTable(this);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SFX_Trash = GameManager.Instance.SFXManager.GetResource("Trash");
        VFX_Clean = GameManager.Instance.VFXManager.GetResource("VFX_Clean");
        var obj = Instantiate(VFX_Clean,VFXPos);
        obj.transform.localPosition = Vector3.zero;
        VFX_Clean = obj.GetComponent<ParticleSystem>();
    }

    public void OnEndEatingTable(Customer customer)
    {
        Trash.SetActive(true);
        InstanceMoney(customer);
        SetChair(false);
    }
    public void OnCleanTable()
    {
        Trash.SetActive(false);
        IsAvailable = true;
        VFX_Clean.Play();
        PlayTrashSFX();
        SetChair(true);
    }

    private void PlayTrashSFX()
    {
        audioSource.clip = SFX_Trash;
        audioSource.Play();
    }

    private void InstanceMoney(Customer customer)
    {
        moneyManager.InstanceMoney(customer.GetPriceToPay() + 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            if (Trash.activeSelf == true)
                OnCleanTable();
        }
    }
    private void SetChair(bool isClean)
    {
        float targetRotY = isClean ? 0 : 40;
        Vector3 targetRot = new Vector3(0, targetRotY, 0);
        ChairPos.parent.localRotation = Quaternion.Euler(targetRot);
    }
}
