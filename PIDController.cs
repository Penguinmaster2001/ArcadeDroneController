using Godot;

public class PIDController
{
    public float Kp { get; set; }
    public float Ki { get; set; }
    public float Kd { get; set; }
    public float IntegralLimit { get; set; }
    public float OutputMin { get; set; }
    public float OutputMax { get; set; }
    public float DerivativeFilter { get; set; }

    private float integral;
    private float previousError;
    private float previousDerivative;

    public PIDController(float kp, float ki, float kd, float integralLimit, float outputMin, float outputMax, float derivativeFilter)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
        IntegralLimit = integralLimit;
        OutputMin = outputMin;
        OutputMax = outputMax;
        DerivativeFilter = derivativeFilter;

        integral = 0.0f;
        previousError = 0.0f;
        previousDerivative = 0.0f;
    }



    public float Update(float target, float actual, float deltaTime)
    {
        float error = target - actual;

        return Update(error, deltaTime);
    }



    public float Update(float error, float deltaTime)
    {
        // Integral term calculation with anti-windup
        integral += error * deltaTime;
        integral = Mathf.Clamp(integral, -IntegralLimit, IntegralLimit);

        // Derivative term calculation with filtering
        float derivative = (error - previousError) / deltaTime;
        derivative = DerivativeFilter * previousDerivative + (1 - DerivativeFilter) * derivative;
        previousDerivative = derivative;

        previousError = error;

        // PID output calculation
        float output = (Kp * error) + (Ki * integral) + (Kd * derivative);

        // Clamp output to specified limits
        output = Mathf.Clamp(output, OutputMin, OutputMax);

        return output;
    }



    public void Reset()
    {
        integral = 0.0f;
        previousError = 0.0f;
        previousDerivative = 0.0f;
    }
}