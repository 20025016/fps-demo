using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f; // 8 units per second
    public float gravity = -20f;
    CharacterController controller;

    private Vector3 _inputVector;

    [Header("Looking")]
    public Transform lookCamera;
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    public float minY = -90;
    public float maxY = 90;

    private float _currentYRotation;

    Vector2 aimVector;

    [Header("Shooting")]
    public float shootRange = 500f;
    public LayerMask shootMask;
    public float fireRate = 0.3f;
    private bool _firing = false;
    public Transform muzzle;
    public GameObject hitEffectPrefab;
    public GameObject bulletPrefab;
    public GameObject muzzleFlash;
    public GameObject gun;
    private Animator _gunAnimator;
    private AudioSource _gunAudio;

    public int maxAmmo = 30;

    private int _currentAmmo = 0;

    public TextMeshProUGUI ammoText;

    // Start is called before the first frame update
    private void Start()
    {
        _gunAnimator = gun.GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        _gunAudio = gun.GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        _currentAmmo = maxAmmo;
        ammoText.text = _currentAmmo.ToString() + "/" + maxAmmo.ToString() + " - Ammo";
    }

    // Update is called once per frame
    private void Update()
    {
        GetInput();
        Move();
        Look();
    }

    private void GetInput()
    {
        // x = LOCAL right and left
        // z = LOCAL forward and back
        // y = LOCAL up and down

        _inputVector.x = Input.GetAxis("Horizontal");
        _inputVector.z = Input.GetAxis("Vertical");

        aimVector.x = Input.GetAxis("Mouse X");
        aimVector.y = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Fire1") && !_firing)
        {
            Shoot();
        }
    }

    private void Move()
    {
        // unit vector = vector with magnitude of 1
        // normalising = reduce a vector's magnitude to 1 while keep the same direction
        
        Vector3 moveVector = transform.TransformDirection(_inputVector.normalized);
        moveVector *= moveSpeed;
        moveVector.y = gravity;
        moveVector *= Time.deltaTime;// use delta time to make things per second rather than per frame
        controller.Move(moveVector);
    }

    private void Look()
    {
        transform.Rotate(transform.up, aimVector.x * sensitivityX);

        _currentYRotation += aimVector.y * sensitivityY;

        _currentYRotation = Mathf.Clamp(_currentYRotation, minY, maxY);

        lookCamera.eulerAngles = new Vector3(-_currentYRotation, lookCamera.eulerAngles.y, lookCamera.eulerAngles.z);
    }

    private void Shoot()
    {
        if (_currentAmmo >= 1)
        {
            _gunAudio.Play();
            _gunAnimator.SetTrigger("Shoot");
            Instantiate(bulletPrefab, muzzle.position, Quaternion.identity, muzzle).transform.forward = muzzle.forward;
            RaycastHit hit;
            if (Physics.Raycast(lookCamera.position, lookCamera.forward, out hit, shootRange, shootMask))
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.identity).transform.forward = hit.transform.TransformDirection(hit.normal);
            }
            Instantiate(muzzleFlash, muzzle.position, Quaternion.identity, muzzle).transform.forward = muzzle.forward;
            StartCoroutine(FireRoutine());
            _currentAmmo--;
            ammoText.text = _currentAmmo.ToString() + "/" + maxAmmo.ToString() + " - Ammo";
        }
    }

    IEnumerator FireRoutine()
    {
        _firing = true;
        yield return new WaitForSeconds(fireRate);
        _firing = false;
    }

    private void OnTriggerEnter(Collider other)// the frame in which a trigger's collider insects with our own
    {
        if (other.CompareTag("Pickup") && _currentAmmo < maxAmmo)
        {
            other.GetComponent<AmmoPickup>().Pickup(out int newAmmo);
            _currentAmmo += newAmmo;
            _currentAmmo = Mathf.Clamp(_currentAmmo, 0, maxAmmo);

            ammoText.text = _currentAmmo.ToString() + "/" + maxAmmo.ToString() + " - Ammo";
        }
    }
}
