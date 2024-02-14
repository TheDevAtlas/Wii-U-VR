using System;
using UnityEngine;

public class MadgwickAHRS
{
	public float Beta { get; set; }
	public float SamplePeriod { get; private set; }
	private Quaternion quaternion;

	public Quaternion Quaternion
	{
		get { return quaternion; }
		private set { quaternion = value; }
	}

	public MadgwickAHRS(float samplePeriod, float beta)
	{
		SamplePeriod = samplePeriod;
		Beta = beta;
		quaternion = Quaternion.identity;
	}

	public void Update(float gx, float gy, float gz, float ax, float ay, float az, float mx, float my, float mz)
	{
		float q1 = quaternion.x, q2 = quaternion.y, q3 = quaternion.z, q4 = quaternion.w;
		float norm;
		float s1, s2, s3, s4;
		float qDot1, qDot2, qDot3, qDot4;

		// Auxiliary variables to avoid repeated arithmetic
		float _2q1mx, _2q1my, _2q1mz, _2q2mx, _2bx = 0, _2bz = 0, _4bx = 0, _4bz = 0, _2q1, _2q2, _2q3, _2q4, _2q1q3, _2q3q4, q1q1, q1q2, q1q3, q1q4, q2q2, q2q3, q2q4, q3q3, q3q4, q4q4;

		// Normalize accelerometer measurement
		norm = (float)Math.Sqrt(ax * ax + ay * ay + az * az);
		if (norm == 0f) return; // handle NaN
		norm = 1 / norm;
		ax *= norm;
		ay *= norm;
		az *= norm;

		// Normalize magnetometer measurement
		norm = (float)Math.Sqrt(mx * mx + my * my + mz * mz);
		if (norm == 0f) return; // handle NaN
		norm = 1 / norm;
		mx *= norm;
		my *= norm;
		mz *= norm;

		// Reference direction of Earth's magnetic field
		_2q1mx = 2f * q1 * mx;
		_2q1my = 2f * q1 * my;
		_2q1mz = 2f * q1 * mz;
		_2q2mx = 2f * q2 * mx;
		_2q1 = 2f * q1;
		_2q2 = 2f * q2;
		_2q3 = 2f * q3;
		_2q4 = 2f * q4;
		_2q1q3 = 2f * q1 * q3;
		_2q3q4 = 2f * q3 * q4;
		q1q1 = q1 * q1;
		q1q2 = q1 * q2;
		q1q3 = q1 * q3;
		q1q4 = q1 * q4;
		q2q2 = q2 * q2;
		q2q3 = q2 * q3;
		q2q4 = q2 * q4;
		q3q3 = q3 * q3;
		q3q4 = q3 * q4;
		q4q4 = q4 * q4;

		// Gradient descent algorithm corrective step
		s1 = -_2q3 * (2f * q2q4 - _2q1q3 - ax) + _2q2 * (2f * q1q2 + _2q3q4 - ay) - _2bz * q3 * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (-_2bx * q4 + _2bz * q2) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + _2bx * q3 * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
		s2 = _2q4 * (2f * q2q4 - _2q1q3 - ax) + _2q1 * (2f * q1q2 + _2q3q4 - ay) - 4f * q2 * (1 - 2f * q2q2 - 2f * q3q3 - az) + _2bz * q4 * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (_2bx * q3 + _2bz * q1) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + (_2bx * q4 - _4bz * q2) * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
		s3 = -_2q1 * (2f * q2q4 - _2q1q3 - ax) + _2q4 * (2f * q1q2 + _2q3q4 - ay) - 4f * q3 * (1 - 2f * q2q2 - 2f * q3q3 - az) + (-_4bx * q3 - _2bz * q1) * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (_2bx * q2 + _2bz * q4) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + (_2bx * q1 - _4bz * q3) * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
		s4 = _2q2 * (2f * q2q4 - _2q1q3 - ax) + _2q3 * (2f * q1q2 + _2q3q4 - ay) + (-_4bx * q4 + _2bz * q2) * (_2bx * (0.5f - q3q3 - q4q4) + _2bz * (q2q4 - q1q3) - mx) + (-_2bx * q1 + _2bz * q3) * (_2bx * (q2q3 - q1q4) + _2bz * (q1q2 + q3q4) - my) + _2bx * q2 * (_2bx * (q1q3 + q2q4) + _2bz * (0.5f - q2q2 - q3q3) - mz);
		norm = 1f / (float)Math.Sqrt(s1 * s1 + s2 * s2 + s3 * s3 + s4 * s4); // normalize step magnitude
		s1 *= norm;
		s2 *= norm;
		s3 *= norm;
		s4 *= norm;

		// Apply feedback step
		qDot1 = 0.5f * (-q2 * gx - q3 * gy - q4 * gz) - Beta * s1;
		qDot2 = 0.5f * (q1 * gx + q3 * gz - q4 * gy) - Beta * s2;
		qDot3 = 0.5f * (q1 * gy - q2 * gz + q4 * gx) - Beta * s3;
		qDot4 = 0.5f * (q1 * gz + q2 * gy - q3 * gx) - Beta * s4;

		// Integrate rate of change of quaternion to yield quaternion
		q1 += qDot1 * SamplePeriod;
		q2 += qDot2 * SamplePeriod;
		q3 += qDot3 * SamplePeriod;
		q4 += qDot4 * SamplePeriod;

		// Normalize quaternion
		norm = 1f / (float)Math.Sqrt(q1 * q1 + q2 * q2 + q3 * q3 + q4 * q4);
		quaternion.x = q1 * norm;
		quaternion.y = q2 * norm;
		quaternion.z = q3 * norm;
		quaternion.w = q4 * norm;
	}
}
