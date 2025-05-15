using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField] private float range;
    private bool pickupActivated = false;
    private RaycastHit hitinfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private Inventory theInventory;
    [SerializeField] private TextMeshProUGUI ResearchLogCountText;
    [SerializeField] private GameObject actionSlider;

    public GameObject player;
    private void Update()
    {
        CheakItem();
        TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed");
            CheakItem();
            if (hitinfo.transform != null)
            {
                Debug.Log("Attempting interaction with object: " + hitinfo.transform.name + ", Tag: " + hitinfo.transform.tag);
                if (hitinfo.transform.CompareTag("Item"))
                    CanPickUp();
                else if (hitinfo.transform.CompareTag("Cabinet"))
                    HitCabinet();
                else if (hitinfo.transform.CompareTag("SubmissionMachine"))
                    Submission();
                else if (hitinfo.transform.CompareTag("CraftingTable"))
                    hitinfo.transform.GetComponent<CraftingTable>().Interact();
                else if (hitinfo.transform.CompareTag("Launch"))
                {
                    // 오디오 재생
                    AudioSource audio = hitinfo.transform.GetComponent<AudioSource>();
                    if (audio != null)
                    {
                        audio.Play();
                    }
                    else
                    {
                        Debug.LogWarning("Launch 오브젝트에 AudioSource가 없습니다: " + hitinfo.transform.name);
                    }

                    // 씬 상태 변경
                    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "2.Planet")
                        GameManager.Instance.ChangeCurrentState(SceneState.Result);
                    else
                        GameManager.Instance.ChangeCurrentState(SceneState.Play);
                }
                else if (hitinfo.transform.CompareTag("SendOff")) {
                    // 전송 시스템
                    SendOff(hitinfo.transform);
                }
                else
                    Debug.Log("E key pressed on unrecognized tag: " + hitinfo.transform.tag);
            }
            else
            {
                Debug.Log("E key pressed but no object was hit");
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (hitinfo.transform != null)
            {
                if (hitinfo.transform.CompareTag("Portal"))
                {
                    Teleport(hitinfo.transform.gameObject);
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            actionSlider.GetComponent<Slider>().value = 0;
        }
    }

    private void Teleport(GameObject hit)
    {
        StartCoroutine(ProgressEnterDungeon(hit));
    }

    private IEnumerator ProgressEnterDungeon(GameObject hit)
    {
        float value = 0f;
        value += Time.deltaTime * 60f;

        if (actionSlider.GetComponent<Slider>().value >= 100f)
        {
            yield return null;
            actionSlider.GetComponent<Slider>().value = 0;
            hit.GetComponent<Teleport>().TeleportDestinationPosition(player);
            player.GetComponentInChildren<FirstPersonController>().transform.localPosition = Vector3.zero;
        }
        else
        {
            actionSlider.GetComponent<Slider>().value += value;
        }
    }


    private void CanPickUp()
    {
        if (pickupActivated)
        {
            ItemPickUp itemPickUp = hitinfo.transform.GetComponent<ItemPickUp>();
            if (itemPickUp == null)
            {
                Debug.LogWarning("ItemPickUp 컴포넌트를 찾을 수 없습니다 on: " + hitinfo.transform.name);
                return;
            }

            if (itemPickUp.Item.itemType == Item.ItemType.Equipment)
            {
                WeaponPickUp wp = hitinfo.transform.GetComponent<WeaponPickUp>();
                if (wp != null)
                    wp.PickUpWeapon(gameObject);
                else
                    Debug.LogWarning("WeaponPickUp 컴포넌트를 찾을 수 없습니다 on: " + hitinfo.transform.name);
            }
            else
            {
                bool acquired = theInventory.AcquireItem(itemPickUp.Item);
                if (acquired)
                {
                    Debug.Log(itemPickUp.Item.itemName + " 획득됨 (인벤토리 추가)");
                    Destroy(hitinfo.transform.gameObject);
                    InfoDisappear();
                }
                else
                {
                    Debug.Log("인벤토리 가득 참: " + itemPickUp.Item.itemName);
                }
            }
        }
    }

    private void HitCabinet()
    {
        Cabinet cabinet = hitinfo.transform.GetComponent<Cabinet>();
        if (cabinet != null)
        {
            cabinet.OpenCabinet();
            InfoDisappear();
        }
        else
        {
            Debug.LogWarning("Cabinet 컴포넌트를 찾을 수 없습니다: " + hitinfo.transform.name);
        }
    }

    private void CheakItem()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitinfo, range, layerMask))
        {
            if (hitinfo.transform.tag == "Item")
                ItemInfoAppear();
            else if (hitinfo.transform.CompareTag("Cabinet"))
                CabinetInfoAppear();
            else if (hitinfo.transform.CompareTag("SubmissionMachine"))
                SubmissionMachineInfo();
            else if (hitinfo.transform.CompareTag("CraftingTable"))
                CraftingTableInfo();
            else if (hitinfo.transform.CompareTag("Launch"))
                LaunchInfo();
            else if (hitinfo.transform.CompareTag("Portal"))
                PortalInfo();
            else if (hitinfo.transform.CompareTag("SendOff"))
                SendOffInfo();
            else
                InfoDisappear();
        }
        else
        {
            InfoDisappear();
        }
    }

    private void SendOffInfo()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = "<color=white>Submit</color> : " + "<color=yellow>(E)</color>";
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = "<color=white>" + hitinfo.transform.GetComponent<ItemPickUp>().Item.itemName + "</color>" +  "<color=yellow>" + "(E)" + "</color>";
    }

    private void CabinetInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = "Open Cabinet: " + "<color=yellow>(E)</color>";
    }

    private void SubmissionMachineInfo()
    {
        actionText.gameObject.SetActive(true);
        actionText.text = "Submission: " + "<color=yellow>(E)</color>";
    }

    private void CraftingTableInfo()
    {
        actionText.gameObject.SetActive(true);
        actionText.text = "Crafting Table: " + "<color=yellow>(E)</color>";
    }

    private void LaunchInfo()
    {
        actionText.gameObject.SetActive(true);
        actionText.text = "Launch: " + "<color=yellow>(E)</color>";
    }
    private void PortalInfo()
    {
        actionText.gameObject.SetActive(true);
        actionText.text = "<color=white>Portal</color> : " + "<color=yellow>(E)</color>";
        actionSlider.gameObject.SetActive(true);
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
        actionSlider.gameObject.SetActive(false);
    }

    private void Submission()
    {
        Debug.Log("제출하기");
        SendOffManager.Instance.SendAllObjects();
        /*

        int totalResearchLogCount = 0;
        int totalSpoilsCount = 0;
        Slot[] slots = theInventory.GetSlots();
        foreach (Slot slot in slots)
        {
            if (slot.item != null)
            {
                if (slot.item.itemType == Item.ItemType.ResearchLog)
                    totalResearchLogCount += slot.itemCount;
                else if (slot.item.itemType == Item.ItemType.Spoils)
                    totalSpoilsCount += slot.itemCount;
            }
        }

        int remainingResearchLog = GameManager.Instance.totalTargetQuota1 - GameManager.Instance.collectedTargetQuota1;
        int remainingSpoils = GameManager.Instance.totalTargetQuota2 - GameManager.Instance.collectedTargetQuota2;
        int allowedSubmissionResearchLog = Mathf.Min(totalResearchLogCount, remainingResearchLog);
        int allowedSubmissionSpoils = Mathf.Min(totalSpoilsCount, remainingSpoils);

        int toRemoveResearchLog = allowedSubmissionResearchLog;
        int toRemoveSpoils = allowedSubmissionSpoils;

        int all_itemValue = 0;

        foreach (Slot slot in slots)
        {
            if (slot.item != null)
            {
                if (slot.item.itemType == Item.ItemType.ResearchLog && toRemoveResearchLog > 0)
                {
                    all_itemValue += Mathf.RoundToInt(slot.item.itemValue);

                    int removeCount = Mathf.Min(slot.itemCount, toRemoveResearchLog);
                    InventoryManager.Instance.RemoveItem(slot.item, removeCount);
                    if (slot.itemCount <= removeCount)
                        slot.ClearSlot();
                    else
                        slot.SetSlotCount(-removeCount);
                    toRemoveResearchLog -= removeCount;
                }
                else if (slot.item.itemType == Item.ItemType.Spoils && toRemoveSpoils > 0)
                {
                    int removeCount = Mathf.Min(slot.itemCount, toRemoveSpoils);
                    InventoryManager.Instance.RemoveItem(slot.item, removeCount);
                    if (slot.itemCount <= removeCount)
                        slot.ClearSlot();
                    else
                        slot.SetSlotCount(-removeCount);
                    toRemoveSpoils -= removeCount;
                }
            }
        }

        GameManager.Instance.collectedTargetQuota1 += allowedSubmissionResearchLog;
        GameManager.Instance.collectedTargetQuota2 += allowedSubmissionSpoils;

        GameManager.Instance.currentQuota += all_itemValue;

        theInventory.RefreshUI();

        Debug.Log("제출 완료 - ResearchLog 제출: " + allowedSubmissionResearchLog + "개, Spoils 제출: " + allowedSubmissionSpoils + "개");
        */
    }

    private void SendOff(Transform parent)
    {
        // 전송 시스템 상호작용
        Slot[] slots = theInventory.GetSlots();
        Slot selectedSlot = slots[theInventory.SelectedIndex];

        BoxCollider range = parent.GetComponent<BoxCollider>();

        float rangeX = range.bounds.size.x;
        float rangeZ = range.bounds.size.z;

        rangeX = Random.Range((rangeX / 2) * -1, rangeX / 2);
        rangeZ = Random.Range((rangeZ / 2) * -1, rangeZ / 2);
        Vector3 RandomPostion = new Vector3(rangeX, 0f, rangeZ);

        if (selectedSlot.item != null)
        {
            GameObject droppedItem = Instantiate(selectedSlot.item.dropPrefab);
            Destroy(droppedItem.GetComponent<Rigidbody>());
            
            droppedItem.transform.SetParent(parent);
            droppedItem.transform.localPosition = RandomPostion;

            SendOffManager.Instance.AddSendObjects(droppedItem);

            if (selectedSlot.itemCount > 1)
            {
                selectedSlot.SetSlotCount(-1);
                InventoryManager.Instance.RemoveItem(selectedSlot.item, 1);
            }
            else
            {
                InventoryManager.Instance.RemoveItem(selectedSlot.item, 1);
                selectedSlot.ClearSlot();
            }
            theInventory.RefreshUI();
        }
    }
}
