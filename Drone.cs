using Godot;
using System;

public partial class Drone : RigidBody3D
{
	[Export]
	private float maxRoll;


	[Export]
	private float rollAuthority;



	[Export]
	private float maxPitch;


	[Export]
	private float pitchAuthority;



	[Export]
	private float yawAuthority;


	[Export]
	private float yawSpeed;



	[Export]
	private float throttle;



	[Export]
	private float maxVerticalSpeed;



	[Export]
	private float motorPower;



	[Export]
	private ProgressBar throttleUI;



	[Export]
	private ProgressBar droneCamera;



	private PIDController pitchController;
	private PIDController rollController;
	private PIDController yawController;

	private PIDController verticalSpeedController;



	private Transform3D homeTransform;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pitchController = new(1.0f, 0.0f, 0.1f, pitchAuthority, -pitchAuthority, pitchAuthority, 0.5f);
		rollController = new(1.0f, 0.0f, 0.1f, rollAuthority, -rollAuthority, rollAuthority, 0.5f);
		yawController = new(1.0f, 0.0f, 0.1f, yawAuthority, -yawAuthority, yawAuthority, 0.5f);

		verticalSpeedController = new(1.0f, 0.0f, 0.5f, maxVerticalSpeed, -maxVerticalSpeed, maxVerticalSpeed, 0.5f);

		homeTransform = Transform;
	}



	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Reset"))
		{
			LinearVelocity = Vector3.Zero;
			AngularVelocity = Vector3.Zero;
			Transform = homeTransform;
		}
	}



    public override void _PhysicsProcess(double delta)
    {
		float deltaFloat = (float) delta;

		// Vector3 rawInput = GetJoystickInput();

		float pitchInput = Input.GetAxis("Backward", "Forward");
		float rollInput = Input.GetAxis("Left", "Right");
		float yawInput = -Input.GetAxis("YawLeft", "YawRight");


		float pitchTarget = pitchInput * maxPitch;
		float yawTarget = yawInput * yawSpeed;
		float rollTarget = rollInput * maxRoll;

		float pitchActual = RotationDegrees.X;
		float yawActual = Mathf.RadToDeg(AngularVelocity.Y) / yawAuthority;
		float rollActual = RotationDegrees.Z;

		float pitchControl = pitchController.Update(pitchTarget, pitchActual, deltaFloat);
		float yawControl = yawController.Update(yawTarget, yawActual, deltaFloat);
		float rollControl = rollController.Update(rollTarget, rollActual, deltaFloat);

		Vector3 controlTorque = Mathf.DegToRad(Mathf.Clamp(pitchControl, -pitchAuthority, pitchAuthority)) * Transform.Basis.X
							  + Mathf.DegToRad(Mathf.Clamp(yawControl, -yawAuthority, yawAuthority)) * Transform.Basis.Y
							  + Mathf.DegToRad(Mathf.Clamp(rollControl, -rollAuthority, rollAuthority)) * Transform.Basis.Z;

		ApplyTorque(controlTorque);


		// Calculate the thrust required to cancel out gravity
		float tiltAngle = Transform.Basis.Y.AngleTo(Vector3.Up);

		GD.Print(tiltAngle);

		float steadyStateThrottle = Mass * 9.81f / (Mathf.Cos(tiltAngle) * motorPower);

		GD.Print(steadyStateThrottle);


		float verticalSpeedInput = Input.GetAxis("ThrottleDown", "ThrottleUp");

		float verticalSpeedTarget = verticalSpeedInput * maxVerticalSpeed;

		float verticalSpeedActual = LinearVelocity.Y / maxVerticalSpeed;

		float verticalSpeedControl = verticalSpeedController.Update(verticalSpeedTarget, verticalSpeedActual, deltaFloat) + steadyStateThrottle;

		// throttle += throttleSpeed * deltaFloat * Input.GetAxis("ThrottleDown", "ThrottleUp");
		throttle = Mathf.Clamp(verticalSpeedControl, 0.0f, 1.0f);

		ApplyCentralForce(throttle * motorPower * Transform.Basis.Y);

		throttleUI.Value = throttle;


		GD.Print($"Input: {pitchInput:F2}, {yawInput:F2}, {rollInput:F2}, {verticalSpeedInput:F2}");
		GD.Print($"Target: {pitchTarget:F2}, {yawTarget:F2}, {rollTarget:F2}, {verticalSpeedTarget:F2}");
		GD.Print($"Control: {pitchControl:F2}, {yawControl:F2}, {rollControl:F2}, {verticalSpeedControl:F2}");
		GD.Print($"Actual: {pitchActual:F2}, {yawActual:F2}, {rollActual:F2}, {verticalSpeedActual:F2}");
		GD.Print();
    }
}
