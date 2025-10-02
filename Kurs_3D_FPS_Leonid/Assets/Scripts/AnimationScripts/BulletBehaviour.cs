using System.Collections;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] public float _speed = 200000f;
    [SerializeField] public float _lifeTime = 10f;

    public Vector3 Direction;

    int i = 100;

    int i_now = 0;

  //  Vector3 _startPosition; 

    void Start()
    {
        StartCoroutine(DelayedDestroy());
    }

    void Update()
    {
        transform.position += (transform.forward * -1) * _speed * Time.deltaTime;
        ////if (Direction == Vector3.zero)
        ////{
        ////    Direction = transform.forward;
        ////}

        ////var pos = new Vector3
        ////{
        ////    x = transform.forward.x + 110,// + 180,
        ////    y = transform.forward.y,
        ////    z = transform.forward.z// + 180
        ////}.normalized;

        ////i_now++;

        ////var pos = Direction.normalized; // * -1

        //var pos = new Vector3
        //{
        //    x = Direction.x + 95,
        //    y = Direction.y,
        //    z = Direction.z// + 180
        //}.normalized;

        //if (i_now > i)
        //{
        //    //
        //}
        //else
        //{
        //    //var pos = Direction.normalized; // * -1
        //    transform.position += pos * _speed * Time.deltaTime;
        //}

        //Debug.DrawLine(transform.position, transform.parent.position, Color.green);
        //Debug.DrawLine(transform.position, Direction, Color.red);

        ////var pos = Direction.normalized; // * -1
        ////transform.position += pos * _speed * Time.deltaTime;
        //// transform.position += transform.forward * _speed * Time.deltaTime; //transform.forward
    }


    private void AutoDestroy()
    {
        Destroy(gameObject);
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(_lifeTime);
        AutoDestroy();
    }
}
