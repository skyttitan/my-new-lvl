using UnityEngine;
using System.Collections;

namespace Kit.Extend
{
	public static class MatrixExtend
    {
		public static Matrix4x4 ToMatrix(this Transform transform, bool local = false)
		{
			Matrix4x4 matrix = new Matrix4x4();
			if (local)
				matrix.SetTRS(transform.localPosition, transform.localRotation, transform.localScale);
			else
				matrix.SetTRS(transform.position, transform.rotation, transform.localScale);
			return matrix;
		}

		public static void ToTransform(this Matrix4x4 matrix, ref Transform transform, bool local = false)
		{
			if (local)
			{
				transform.localPosition = matrix.GetPosition();
				transform.localRotation = matrix.GetRotation();
			}
			else
			{
				transform.position = matrix.GetPosition();
				transform.rotation = matrix.GetRotation();
			}
			transform.localScale = matrix.GetScale();
		}

        public static Vector3 GetPosition(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

		/// <summary>Get Local scale of matrix</summary>
		/// <param name="matrix"></param>
		/// <returns></returns>
		/// <remarks>Not able to get negative scale</remarks>
		/// <see cref="http://answers.unity3d.com/questions/402280/how-to-decompose-a-trs-matrix.html"/>
		/// <seealso cref="http://forum.unity3d.com/threads/benchmarking-mathf-with-system-math-big-performance-differences.194938/"/>
		public static Vector3 GetScale(this Matrix4x4 matrix)
        {
            Vector3 scale = new Vector3(
                matrix.GetColumn(0).magnitude,
                matrix.GetColumn(1).magnitude,
                matrix.GetColumn(2).magnitude);
			if (Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)).normalized != (Vector3)matrix.GetColumn(2).normalized)
			{
				scale.x *= Mathf.Sign(-1);
			}
			return scale;
		}

#if true
        /// <summary>Get Local rotation of matrix</summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        /// <see cref="http://forum.unity3d.com/threads/is-it-possible-to-get-a-quaternion-from-a-matrix4x4.142325/"/>
        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
			Vector4
				lhs = matrix.GetColumn(2),
				rhs = matrix.GetColumn(1);
			if (lhs == Vector4.zero && rhs == Vector4.zero)
				return Quaternion.identity;
			else
				return Quaternion.LookRotation(lhs, rhs);
        }
#else
        /// <summary>Get Rotation</summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        /// <see cref="http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm"/>
        /// <seealso cref="http://answers.unity3d.com/questions/11363/converting-matrix4x4-to-quaternion-vector3.html"/>
        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
            q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
            q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
            q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
            return q;
        }
#endif
	}
}
