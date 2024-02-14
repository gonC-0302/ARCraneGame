using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraneController : MonoBehaviour
{
    private enum State
    {
        Standby,
        MoveRight,
        MoveBack,
        MoveOrigin
    }

    /// <summary>
    /// クレーン
    /// </summary>
    private Vector3 _craneOriginPos;
    private Animator _anim;
    private Rigidbody _craneRb;
    const float _speed = 0.1f;
    private State _currentState;

    /// <summary>
    /// ボタン
    /// </summary>
    [SerializeField] private GameObject _rightButton, _backButton;

    /// <summary>
    /// 仮想の爪
    /// </summary>
    [SerializeField] private GameObject _virtualClawPrefab;
    [SerializeField] private Transform _virtualClawTran;
    private GameObject _virtualClaw;
    private Rigidbody _virtualClawRb;

    private void Start()
    {
        _craneOriginPos = transform.localPosition;
        _anim = gameObject.GetComponent<Animator>();
        _anim.enabled = false;
        _craneRb = gameObject.GetComponent<Rigidbody>();
        _currentState = State.Standby;
    }

    private void Update()
    {
        // 右へ移動
        if(_currentState == State.MoveRight)
        {
            transform.position += _speed * transform.right * Time.deltaTime;
        }
        // 奥へ移動
        else if(_currentState == State.MoveBack)
        {
            transform.position += _speed * transform.forward * Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // クレーンを初期位置へ戻す
        if(_currentState == State.MoveOrigin)
        {
            var vec = _craneOriginPos - transform.localPosition;
            var dis = vec.magnitude;
            if (dis <= 0.1) OnArrivedOriginPos();
            var vec_normarized = vec.normalized;
            _virtualClawRb.MovePosition(_virtualClaw.transform.position + vec_normarized * Time.deltaTime * _speed);
            _craneRb.MovePosition(transform.position + vec_normarized * Time.deltaTime * _speed);
        }
    }

    /// <summary>
    /// クレーンが閉じて上昇し始める時に生成（AnimationEvent）
    /// </summary>
    public void SpawnVirtualCrane()
    {
        _virtualClaw = Instantiate(_virtualClawPrefab, _virtualClawTran);
        _virtualClawRb = _virtualClaw.GetComponent<Rigidbody>();
    }
    /// <summary>
    /// 初期位置へ移動フラグ（AnimationEvent）
    /// </summary>
    public void MoveOriginPos()
    {
        _currentState = State.MoveOrigin;
        _virtualClaw.transform.SetParent(null);
    }
    private void OnArrivedOriginPos()
    {
        _currentState = State.Standby;
        Destroy(_virtualClaw,0.25f);
        _anim.SetTrigger("Open");
        _rightButton.SetActive(true);
    }

    /// <summary>
    /// 右ボタン押下
    /// </summary>
    public void OnRightButtonDown()
    {
        if (_currentState != State.Standby) return;
        _currentState = State.MoveRight;
        ChangeButtonColor(_rightButton, true);
    }
    public void OnRightButtonUp()
    {
        ChangeButtonColor(_rightButton, false);
        _currentState = State.Standby;
        _rightButton.SetActive(false);
        _backButton.SetActive(true);
    }
    /// <summary>
    /// 奥ボタン押下
    /// </summary>
    public void OnBackButtonDown()
    {
        if (_currentState != State.Standby) return;
        _currentState = State.MoveBack;
        ChangeButtonColor(_backButton, true);
    }
    public void OnBackButtonUp()
    {
        _currentState = State.Standby;
        _anim.enabled = true;
        ChangeButtonColor(_backButton, false);
        _backButton.SetActive(false);
        _anim.Play("Catch",0,0);
    }
    private void ChangeButtonColor(GameObject btn,bool isDown)
    {
        var img = btn.GetComponent<Image>();
        if (isDown) img.color = Color.green;
        else img.color = Color.white;
    }
}
