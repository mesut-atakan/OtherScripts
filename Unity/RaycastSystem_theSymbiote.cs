using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastSystem : MonoBehaviour
{
    [SerializeField] [Range(0f,20f)] [Tooltip("Ray mesafesi")] private float RayDistance = 10.0f;
    [SerializeField] [Tooltip("Raycast hangi kameradan çıkacak ?")] private Camera cam;

    [SerializeField] private LayerMask layermask;

    [Space(10)]

    [Header("Player Teleport")]
    [SerializeField] [Tooltip("Teleport Speed")] [Range(0f, 1.0f)] private float TeleportSpeed = 0.2f;

    [Space(10)]
    [Header("Teleport Timer")]
    private bool teleportBool;
    [SerializeField] [Tooltip("Crossun etrafındaki load circleın ön plandaki imagini seçeceğiz!")] private Image LoadCircle;
    [SerializeField] [Tooltip("Circle Bacground")] private Image BackGroundCircle;
    private float _currentTime; //mevcut zaman
    [SerializeField] [Tooltip("Teleport için verilecek Süre")] [Range(25f, 125f)] private float NextTeleportTime;

    [SerializeField] private DNA_Password PasswordScript;
    [SerializeField] private GameObject DNAGameObject;

    private void Awake() 
    {

        
        teleportBool = true;
        _currentTime = NextTeleportTime;
        FindLoadCircle();
        LoadCircle.fillAmount = Mathf.InverseLerp(0, NextTeleportTime, _currentTime);
    }

    private void Update() 
    {
        if(Input.GetMouseButtonDown(1))
        {
            var hit = RaySystem();
            if(hit.collider != null)
            {
                hit = RaySystem();
                Debug.Log(hit.collider.tag);
                if(hit.collider.tag == "rival" && teleportBool == true)
                {
                    StartCoroutine(LerpPosition(hit.transform.position, TeleportSpeed, hit));
                    StartCoroutine(UpdateTime());
                }
            }
        }
    }

    IEnumerator UpdateTime()
    {
        while(_currentTime <= NextTeleportTime && _currentTime > 0)
        {
            FindLoadCircle();
            teleportBool = false;
            LoadCircle.fillAmount = Mathf.InverseLerp(0, NextTeleportTime, _currentTime);
            yield return new WaitForSeconds(0.1f);
            _currentTime--;
        }

        if(_currentTime <= 0)
        {
            FindLoadCircle();
            teleportBool = true;
            _currentTime = NextTeleportTime;
            LoadCircle.fillAmount = Mathf.InverseLerp(0, NextTeleportTime, _currentTime);
        }
        yield return null;
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
        playerTargetReach(targetPosition, hit);
    }

    private void playerTargetReach(Vector3 Target, RaycastHit hit)
    {
        if(Vector3.Distance(transform.position, Target) < 0.1f)
        {
            DNAGameObject.SetActive(true);
            //DNA_Script = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<DNA_Password>().DNA_Start();
           PasswordScript.DNA_Start();
           Destroy(hit.collider.gameObject);
        }
    }

    private RaycastHit RaySystem()
    {
        RaycastHit hit;
        Ray ray = CameraReferance.Instance.CurrentCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        Physics.Raycast(ray, out hit, RayDistance, layermask);
        return hit;
    }

    private void FindLoadCircle()
    {
        if (LoadCircle == null)
        {
            LoadCircle = GameObject.Find("load_Circle").GetComponent<Image>();
        }
    }
}

/*https://hopefulgames.itch.io/the-symbiote
this script the-symbiote game raycastSystem 
*/
