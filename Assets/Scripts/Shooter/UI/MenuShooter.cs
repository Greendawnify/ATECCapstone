using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MenuShooter : MonoBehaviour
{
    public string spawnFromPoolTag;

    Camera camera;
    AudioSource source;
    ObjectPooler pooler;
    AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        pooler = ObjectPooler.Instance;
        source = GetComponent<AudioSource>();
        audioManager = AudioManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // if you touch the screen or click it with mouse
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || (Input.GetMouseButtonDown(0)))
        {
            if (Input.touchCount > 0)
            {
                // send touch position info if I touched the screen
                DoRaycast(Input.GetTouch(0).position);
            }
            else
            {
                // send mouse position info if I clicked
                DoRaycast(Input.mousePosition);
            }

        }
    }

    void DoRaycast(Vector2 touch)
    {
        // creates a ray based on where the screen is interacted with
        Ray ray = camera.ScreenPointToRay(new Vector3(touch.x, touch.y, 0f));

        // creates a player bullet and calls a funtion in PlayerBullet script
        //GameObject newObj = Instantiate(bullet, ray.origin, Quaternion.identity);
        GameObject newObj = pooler.SpawnFromPool(spawnFromPoolTag, ray.origin, Quaternion.identity);
        newObj.SetActive(true);
        newObj.GetComponent<PlayerBullet>().SetBulletShooting(ray.direction);

        if (source)
            audioManager.PlayPlayerShoot(source);
    }
}
