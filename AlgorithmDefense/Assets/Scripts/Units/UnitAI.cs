using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAI : MonoBehaviour
{
    public Define.Move MoveType;

    private UnitManager _detectedUnit;
    private Animator _anim;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private int attackDamage;
    [SerializeField] private float _speed = 0f;
    [SerializeField] private float _range = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _anim = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_detectedUnit)
        {
            Move();
        }

        CheckLayer();
    }

    public void InflictDamage()
    {
        bool unitDie = _detectedUnit.LoseHp(attackDamage);

        if (unitDie)
        {
            _anim.SetBool("Attack", false);

            _detectedUnit = null;
        }
    }

    private void Move()
    {
        _anim.SetBool("Attack", false);

        if (MoveType == Define.Move.Down)
        {
            _anim.SetFloat("Hor", 0);
            _anim.SetFloat("Ver", -1);
            transform.Translate(Vector2.down * _speed * Time.deltaTime);
        }
        else
        {
            _anim.SetFloat("Hor", 0);
            _anim.SetFloat("Ver", 1);
            transform.Translate(Vector2.up * _speed * Time.deltaTime);
        }
    }

    private void CheckLayer()
    {
        if (_detectedUnit)
        {
            return;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _range, layerMask);
        
        foreach (Collider2D collider2D in colliders)
        {
            if (colliders != null)
            {
                _detectedUnit = collider2D.GetComponent<UnitManager>();
                _anim.SetBool("Attack", true);
            }
        }
    }
}