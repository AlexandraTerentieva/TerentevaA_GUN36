using System.Collections;
using UnityEngine;

public class AiWeapons : MonoBehaviour
{
    public enum WeaponState
    {
        Holstering,
        Holstered,
        Activating,
        Active,
        Reloading
    }

    public enum WeaponSlot
    {
        Primary,
        Secondary
    }

    public RaycastWeapon currentWeapon
    {
        get
        {
            if (weapons == null || weapons.Length == 0) return null;
            return weapons[current];
        }
    }

    public WeaponSlot currentWeaponSlot
    {
        get
        {
            return (WeaponSlot)current;
        }
    }

    private RaycastWeapon[] weapons = new RaycastWeapon[2];
    private int current = 0;
    private Animator animator;
    private MeshSockets sockets;
    private WeaponIk weaponIk;
    private Transform currentTarget;
    private WeaponState weaponState = WeaponState.Holstered;
    public float inaccuracy = 0.0f;
    public float dropForce = 1.5f;
    private GameObject magazineHand;

    public bool IsActive() => weaponState == WeaponState.Active;
    public bool IsHolstered() => weaponState == WeaponState.Holstered;
    public bool IsReloading() => weaponState == WeaponState.Reloading;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sockets = GetComponent<MeshSockets>();
        weaponIk = GetComponent<WeaponIk>();
    }

    private void Update()
    {
        if (currentTarget != null && currentWeapon != null && IsActive())
        {
            Vector3 target = currentTarget.position + weaponIk.targetOffset;
            target += Random.insideUnitSphere * inaccuracy;
            currentWeapon.UpdateWeapon(Time.deltaTime, target);
        }

        // œ–»Õ”ƒ»“≈À‹ÕŒ ƒŒ¡¿¬Àﬂ≈Ã œ¿“–ŒÕ€  ¿∆ƒ€…  ¿ƒ–
        if (currentWeapon != null)
        {
            currentWeapon.clipCount = 999;
        }
    }

    public void SetFiring(bool enabled)
    {
        if (currentWeapon == null) return;
        if (enabled) currentWeapon.StartFiring();
        else currentWeapon.StopFiring();
    }

    public void DropWeapon()
    {
        if (currentWeapon == null || weapons == null) return;
        currentWeapon.transform.SetParent(null);
        currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
        currentWeapon.gameObject.AddComponent<Rigidbody>();
        weapons[current] = null;
    }

    public bool HasWeapon() => currentWeapon != null;

    public void SetTarget(Transform target)
    {
        if (weaponIk != null)
            weaponIk.SetTargetTransform(target);
        currentTarget = target;
    }

    public void Equip(RaycastWeapon weapon)
    {
        if (weapon == null) return;
        current = (int)weapon.weaponSlot;
        weapons[current] = weapon;
        if (sockets != null)
            sockets.Attach(weapon.transform, weapon.holsterSocket);
    }

    public void ActivateWeapon()
    {
        if (currentWeapon == null) return;
        StartCoroutine(EquipWeaponAnimation());
    }

    public void DeactivateWeapon()
    {
        if (currentWeapon == null) return;
        SetTarget(null);
        SetFiring(false);
        StartCoroutine(HolsterWeaponAnimation());
    }

    public void ReloadWeapon()
    {
        if (currentWeapon == null) return;
        if (IsActive()) StartCoroutine(ReloadWeaponAnimation());
    }

    public void SwitchWeapon(WeaponSlot slot)
    {
        if (weapons == null || weapons[(int)slot] == null) return;

        if (IsHolstered())
        {
            current = (int)slot;
            ActivateWeapon();
            return;
        }

        int equipIndex = (int)slot;
        if (IsActive() && current != equipIndex)
        {
            StartCoroutine(SwitchWeaponAnimation(equipIndex));
        }
    }

    public int Count()
    {
        int count = 0;
        if (weapons == null) return 0;
        foreach (var weapon in weapons)
        {
            if (weapon != null) count++;
        }
        return count;
    }

    private IEnumerator EquipWeaponAnimation()
    {
        if (currentWeapon == null || animator == null) yield break;

        weaponState = WeaponState.Activating;
        animator.runtimeAnimatorController = currentWeapon.animator;
        animator.SetBool("equip", true);
        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }

        if (weaponIk != null)
        {
            weaponIk.enabled = true;
            weaponIk.SetAimTransform(currentWeapon.raycastOrigin);
        }
        weaponState = WeaponState.Active;
    }

    private IEnumerator HolsterWeaponAnimation()
    {
        if (currentWeapon == null || animator == null) yield break;

        weaponState = WeaponState.Holstering;
        animator.SetBool("equip", false);
        if (weaponIk != null) weaponIk.enabled = false;
        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }

        weaponState = WeaponState.Holstered;
    }

    private IEnumerator ReloadWeaponAnimation()
    {
        if (currentWeapon == null || animator == null) yield break;

        weaponState = WeaponState.Reloading;
        animator.SetTrigger("reload_weapon");
        if (weaponIk != null) weaponIk.enabled = false;
        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }

        if (weaponIk != null) weaponIk.enabled = true;
        weaponState = WeaponState.Active;
    }

    private IEnumerator SwitchWeaponAnimation(int index)
    {
        yield return StartCoroutine(HolsterWeaponAnimation());
        current = index;
        yield return StartCoroutine(EquipWeaponAnimation());
    }

    public void OnAnimationEvent(string eventName)
    {
        if (currentWeapon == null) return;

        switch (eventName)
        {
            case "attach_weapon": AttachWeapon(); break;
            case "detach_magazine": DetachMagazine(); break;
            case "drop_magazine": DropMagazine(); break;
            case "refill_magazine": RefillMagazine(); break;
            case "attach_magazine": AttachMagazine(); break;
        }
    }

    private void AttachWeapon()
    {
        if (currentWeapon == null || sockets == null) return;
        bool equipping = animator.GetBool("equip");
        if (equipping)
            sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.RightHand);
        else
            sockets.Attach(currentWeapon.transform, currentWeapon.holsterSocket);
    }

    private void DetachMagazine()
    {
        if (currentWeapon == null) return;
        var leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        magazineHand = Instantiate(currentWeapon.magazine, leftHand, true);
        currentWeapon.magazine.SetActive(false);
    }

    private void DropMagazine()
    {
        if (magazineHand == null) return;
        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMagazine.SetActive(true);
        droppedMagazine.AddComponent<Rigidbody>().AddForce((-gameObject.transform.right + Vector3.down) * dropForce, ForceMode.Impulse);
        droppedMagazine.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);
    }

    private void RefillMagazine()
    {
        if (magazineHand == null) return;
        magazineHand.SetActive(true);
    }

    private void AttachMagazine()
    {
        if (currentWeapon == null || magazineHand == null) return;
        currentWeapon.magazine.SetActive(true);
        Destroy(magazineHand);
        currentWeapon.RefillAmmo();
        animator.ResetTrigger("reload_weapon");
    }

    public void RefillAmmo(int clipCount)
    {
        if (currentWeapon != null)
            currentWeapon.clipCount += clipCount;
    }

    public bool IsLowAmmo()
    {
        return false;
    }
}