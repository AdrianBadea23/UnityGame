using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    
    public float moveSpeed, gravityModifier, jumpPower, runSpeed = 12f;
    public CharacterController charCon;
    public Transform camTransform;
    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;
    private Vector3 moveInput;
    private bool canJump;
    private int numberOfJumps;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;
    public Animator anim;

    //public GameObject bullet;
    public Transform firePoint;
    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> unlockableGuns = new List<Gun>();
    public int currentGun;

    public Transform adsPoint, gunHolder;
    private Vector3 gunStartPos;
    public float adsSpeed = 2f;

    public GameObject muzzleFlash;
    public AudioSource footstepFast, footstepSlow;
    private float bounceAmount;
    private bool bounce;

    public float maxViewAngle = 60f;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentGun--;
        switchGun();
        gunStartPos = gunHolder.localPosition;
        UIController.instance.score.text = "0";

    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.instance.pauseScreen.activeInHierarchy && !GameManager.instance.ending)
        {


            // moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            // moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            //store y velo
            float yStore = moveInput.y;

            Vector3 vertMove = transform.forward * Input.GetAxisRaw("Vertical");
            Vector3 horiMove = transform.right * Input.GetAxisRaw("Horizontal");
            moveInput = horiMove + vertMove;
            moveInput.Normalize();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveInput *= runSpeed;
            }
            else
            {
                moveInput *= moveSpeed;
            }

            moveInput.y = yStore;
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
            if (charCon.isGrounded)
            {
                moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
            }

            canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;

            if (canJump && charCon.isGrounded)
            {
                numberOfJumps = 2;
            }

            //handle jumping
            if (Input.GetKeyDown(KeyCode.Space) && numberOfJumps > 0)
            {
                moveInput.y = jumpPower;
                numberOfJumps--;
                AudioManager.instance.PlaySFX(8);
            }

            if (bounce)
            {
                bounce = false;
                moveInput.y = bounceAmount;
                numberOfJumps = 1;
            }

            charCon.Move(moveInput * Time.deltaTime);

            // Control camera rotation
            Vector2 mouseInput =
                new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

            if (invertX)
            {
                mouseInput.x = -mouseInput.x;
            }

            if (invertY)
            {
                mouseInput.y = -mouseInput.y;
            }

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
            camTransform.rotation = Quaternion.Euler(camTransform.rotation.eulerAngles.x - mouseInput.y,
                camTransform.rotation.eulerAngles.y, camTransform.rotation.eulerAngles.z);

            if (camTransform.rotation.eulerAngles.x > maxViewAngle && camTransform.rotation.eulerAngles.x < 180f)
            {
                camTransform.rotation = Quaternion.Euler(maxViewAngle, camTransform.rotation.eulerAngles.y,
                    camTransform.rotation.eulerAngles.z);
            }else if (camTransform.rotation.eulerAngles.x > 180f &&
                      camTransform.rotation.eulerAngles.x < 360f - maxViewAngle)
            {
                camTransform.rotation = Quaternion.Euler(-maxViewAngle, camTransform.rotation.eulerAngles.y,
                    camTransform.rotation.eulerAngles.z);
            }

            muzzleFlash.SetActive(false);
            //Handle Shooting
            // single shots
            if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 50f))
                {
                    if (Vector3.Distance(camTransform.position, hit.point) > 2f)
                    {
                        firePoint.LookAt(hit.point);
                    }
                }
                else
                {
                    firePoint.LookAt(camTransform.position + (camTransform.forward * 30f));
                }

                // Instantiate(bullet, firePoint.position, firePoint.rotation);
                FireShot();
            }

            // repeating shots
            if (Input.GetMouseButton(0) && activeGun.canAutoFire)
            {
                if (activeGun.fireCounter <= 0)
                {
                    FireShot();
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                switchGun();
            }

            if (Input.GetMouseButtonDown(1))
            {
                CameraController.instance.zoomIn(activeGun.zoomAmount);
            }

            if (Input.GetMouseButton(1))
            {
                gunHolder.position =
                    Vector3.MoveTowards(gunHolder.position, adsPoint.position, adsSpeed * Time.deltaTime);
            }
            else
            {
                gunHolder.localPosition =
                    Vector3.MoveTowards(gunHolder.localPosition, gunStartPos, adsSpeed * Time.deltaTime);

            }

            if (Input.GetMouseButtonUp(1))
            {
                CameraController.instance.zoomOut();
            }

            anim.SetFloat("moveSpeed", moveInput.magnitude);
            anim.SetBool("onGround", canJump);
        }
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;
            Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);
            activeGun.fireCounter = activeGun.fireRate;
            UIController.instance.ammoText.text = "" + activeGun.currentAmmo;
            muzzleFlash.SetActive(true);
        }
        
    }

    public void switchGun()
    {   
        activeGun.gameObject.SetActive(false);
        currentGun++;

        if (currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }
        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);
        UIController.instance.ammoText.text = "" + activeGun.currentAmmo;
        firePoint.position = activeGun.firepoint.position;
    }

    public void addGun(string gunToAdd)
    {
        bool gunUnlocked = false;

        if (unlockableGuns.Count > 0)
        {
            for (int i = 0; i < unlockableGuns.Count; i++)
            {
                if (unlockableGuns[i].gunName == gunToAdd)
                {
                    gunUnlocked = true;
                    allGuns.Add(unlockableGuns[i]);
                    
                    unlockableGuns.RemoveAt(i);

                    i = unlockableGuns.Count;
                }
            }
        }

        if (gunUnlocked)
        {
            currentGun = allGuns.Count - 2;
            switchGun();
        }
    }

    public void Bounce(float bounceForce)
    {
        bounceAmount = bounceForce;
        bounce = true;
    }
}
