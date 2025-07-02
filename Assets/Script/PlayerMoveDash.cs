using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerDataWithDash Data;
    public bool hasKey = false;

    AudioManager audioManager;
    private Animator _animator; // MODIFIKASI: Variabel untuk Animator

    #region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    #endregion

    #region STATE PARAMETERS
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsSliding { get; private set; }

    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    private bool _isJumpCut;
    private bool _isJumpFalling;

    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;
    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    #endregion

    #region DEBUG PARAMETERS (NEW!)
    [Header("DEBUG")]
    [SerializeField] private bool _debugMode = false;
    #endregion

    private bool _isWalkingSFXPlaying = false;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>(); // MODIFIKASI: Dapatkan komponen Animator
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
    }

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.J))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.C) || Input.GetKeyUp(KeyCode.J))
        {
            OnJumpUpInput();
        }

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.K))
        {
            OnDashInput();
        }
        #endregion

        #region SFX LOGIC
        if (LastOnGroundTime > 0 && Mathf.Abs(_moveInput.x) > 0.01f && !IsDashing && !IsJumping && !IsSliding)
        {
            if (!_isWalkingSFXPlaying)
            {
                audioManager.PlayLoopingSFX(audioManager.walk);
                _isWalkingSFXPlaying = true;
            }
        }
        else
        {
            if (_isWalkingSFXPlaying)
            {
                audioManager.StopLoopingSFX();
                _isWalkingSFXPlaying = false;
            }
        }
        #endregion

        #region COLLISION CHECKS
        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping && !IsDashing)
        {
            if (LastOnGroundTime <= 0)
            {
                audioManager.PlaySFX(audioManager.land);
            }
            LastOnGroundTime = Data.coyoteTime;
        }

        if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && !IsFacingRight)) && !IsWallJumping)
        {
            LastOnWallRightTime = Data.coyoteTime;
        }

        if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _wallLayer) && IsFacingRight)) && !IsWallJumping)
        {
            LastOnWallLeftTime = Data.coyoteTime;
        }

        LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        #endregion

        #region JUMP CHECKS
        if (IsJumping && RB.linearVelocity.y < 0)
        {
            IsJumping = false;

            if (!IsWallJumping)
                _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            if (!IsJumping)
                _isJumpFalling = false;
        }

        if (!IsDashing)
        {
            if (CanJump() && LastPressedJumpTime > 0)
            {
                IsJumping = true;
                IsWallJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();
                audioManager.PlaySFX(audioManager.jump);
            }
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                IsWallJumping = true;
                IsJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;

                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

                WallJump(_lastWallJumpDir);
                audioManager.PlaySFX(audioManager.wallJump);
            }
        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && LastPressedDashTime > 0)
        {
            Sleep(Data.dashSleepTime);

            if (_moveInput != Vector2.zero)
                _lastDashDir = _moveInput;
            else
                _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

            IsDashing = true;
            IsJumping = false;
            IsWallJumping = false;
            _isJumpCut = false;

            StartCoroutine(nameof(StartDash), _lastDashDir);
            audioManager.PlaySFX(audioManager.dash);
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            if (IsSliding == false)
            {
                audioManager.PlayLoopingSFX(audioManager.wallSlide);
            }
            IsSliding = true;
        }
        else
        {
            if (IsSliding == true)
            {
                audioManager.StopLoopingSFX();
            }
            IsSliding = false;
        }
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            if (IsSliding)
            {
                SetGravityScale(0);
            }
            else if (RB.linearVelocity.y < 0 && _moveInput.y < 0)
            {
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFastFallSpeed));
            }
            else if (_isJumpCut)
            {
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
            }
            else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.linearVelocity.y < 0)
            {
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
            }
            else
            {
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            SetGravityScale(0);
        }
        #endregion

        // MODIFIKASI: Panggil method untuk update Animator
        UpdateAnimatorParameters();
    }

    private void FixedUpdate()
    {
        if (!IsDashing)
        {
            if (IsWallJumping)
                Run(Data.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (_isDashAttacking)
        {
            Run(Data.dashEndRunLerp);
        }

        if (IsSliding)
            Slide();
    }
    
    // MODIFIKASI: Method baru untuk mengontrol Animator
    private void UpdateAnimatorParameters()
    {
        _animator.SetBool("isGrounded", LastOnGroundTime > 0);
        _animator.SetFloat("horizontalSpeed", Mathf.Abs(RB.linearVelocity.x));
        _animator.SetFloat("yVelocity", RB.linearVelocity.y);
        _animator.SetBool("isSliding", IsSliding);
    }

    #region INPUT CALLBACKS
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }

    public void OnDashInput()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        targetSpeed = Mathf.Lerp(RB.linearVelocity.x, targetSpeed, lerpAmount);

        float accelRate;
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;

        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }

        if (Data.doConserveMomentum && Mathf.Abs(RB.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        float speedDif = targetSpeed - RB.linearVelocity.x;
        float movement = speedDif * accelRate;
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        float force = Data.jumpForce;
        if (RB.linearVelocity.y < 0)
            force -= RB.linearVelocity.y;

        // MODIFIKASI: Panggil trigger lompat untuk Animator
        _animator.SetTrigger("jumpTrigger");
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(RB.linearVelocity.x) != Mathf.Sign(force.x))
            force.x -= RB.linearVelocity.x;

        if (RB.linearVelocity.y < 0)
            force.y -= RB.linearVelocity.y;

        // MODIFIKASI: Panggil trigger lompat untuk Animator
        _animator.SetTrigger("jumpTrigger");
        RB.AddForce(force, ForceMode2D.Impulse);
    }
    #endregion

    #region DASH METHODS
    private IEnumerator StartDash(Vector2 dir)
    {
        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        _dashesLeft--;
        _isDashAttacking = true;

        SetGravityScale(0);

        audioManager.PlaySFX(audioManager.dash);

        while (Time.time - startTime <= Data.dashAttackTime)
        {
            RB.linearVelocity = dir.normalized * Data.dashSpeed;
            yield return null;
        }

        startTime = Time.time;

        _isDashAttacking = false;

        SetGravityScale(Data.gravityScale);
        RB.linearVelocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }

        IsDashing = false;
    }

    private IEnumerator RefillDash(int amount)
    {
        _dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        _dashRefilling = false;
        _dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        float speedDif = Data.slideSpeed - RB.linearVelocity.y;
        float movement = speedDif * Data.slideAccel;
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }
    #endregion


    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
                         (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && RB.linearVelocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.linearVelocity.y > 0;
    }

    private bool CanDash()
    {
        if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return _dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion


    #region DEBUG METHODS
    private void OnGUI()
    {
        if (!_debugMode) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = Color.black;

        GUI.Label(new Rect(10, 10, 200, 30), $"IsFacingRight: {IsFacingRight}", style);
        GUI.Label(new Rect(10, 40, 200, 30), $"IsJumping: {IsJumping}", style);
        GUI.Label(new Rect(10, 70, 200, 30), $"IsWallJumping: {IsWallJumping}", style);
        GUI.Label(new Rect(10, 100, 200, 30), $"IsDashing: {IsDashing}", style);
        GUI.Label(new Rect(10, 130, 200, 30), $"IsSliding: {IsSliding}", style);

        GUI.Label(new Rect(10, 190, 200, 30), $"LastOnGroundTime: {LastOnGroundTime:F2}", style);
        GUI.Label(new Rect(10, 220, 200, 30), $"LastPressedJumpTime: {LastPressedJumpTime:F2}", style);

        GUI.Label(new Rect(10, 250, 300, 30), $"Velocity X: {RB.linearVelocity.x:F2}", style);
        GUI.Label(new Rect(10, 280, 300, 30), $"Velocity Y: {RB.linearVelocity.y:F2}", style);
        GUI.Label(new Rect(10, 310, 200, 30), $"Gravity Scale: {RB.gravityScale:F2}", style);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);

        if (_debugMode)
        {
            Gizmos.color = IsFacingRight ? Color.yellow : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)(IsFacingRight ? Vector2.right : Vector2.left) * 1.5f);
        }
    }
    #endregion
}