
// Rigid Transform implementation using explanation in http://nghiaho.com/?page_id=671

using Accord.Math;
using UnityEngine;

namespace Calibration
{
    public class TriadMarker : MonoBehaviour
    {
        [SerializeField] private Triad _triad;

        private Matrix3x3 _h;
        private Matrix3x3 _u;
        private Accord.Math.Vector3 _e;
        private Matrix3x3 _v;
        private Matrix3x3 _r;
        private Accord.Math.Vector3 _centroidA;
        private Accord.Math.Vector3 _centroidB;
        private Accord.Math.Vector4 _x;
        private UnityEngine.Matrix4x4 _transformationMatrix;
        private Accord.Math.Vector3 _translation;
    
        private UnityEngine.Vector3 _initPosition;
        private Quaternion _initRotation;

        public Triad Triad => _triad;

        
        private void Start ()
        {
            var wrongInitPosition = _triad.Anchor.transform.position;
            _initPosition = new UnityEngine.Vector3(-wrongInitPosition.x, wrongInitPosition.z, wrongInitPosition.y);
            _initRotation = _triad.Anchor.transform.rotation;        
        }
        
        
        public void ApplyTransformation(
            Transform target1StMarkerTransform, 
            Transform target2NdMarkerTransform, 
            Transform target3RdMarkerTransform)
        {
            _centroidA = (UnitytoAccord(_triad.RefMarker1St.transform.position) + 
                         UnitytoAccord(_triad.RefMarker2Nd.transform.position) + 
                         UnitytoAccord(_triad.RefMarker3Rd.transform.position)) / 3;
        
            _centroidB = (UnitytoAccord(target1StMarkerTransform.position) + 
                         UnitytoAccord(target2NdMarkerTransform.position) + 
                         UnitytoAccord(target3RdMarkerTransform.position)) / 3;

            // Calculating Covariance Matrix
            _h = CovarianceMatrixStep(
                    UnitytoAccord(_triad.RefMarker1St.transform.position) - _centroidA, 
                    UnitytoAccord(target1StMarkerTransform.position) - _centroidB) + 
                CovarianceMatrixStep(
                    UnitytoAccord(_triad.RefMarker2Nd.transform.position) - _centroidA, 
                    UnitytoAccord(target2NdMarkerTransform.position) - _centroidB) + 
                CovarianceMatrixStep(
                    UnitytoAccord(_triad.RefMarker3Rd.transform.position) - _centroidA, 
                    UnitytoAccord(target3RdMarkerTransform.position) - _centroidB);

            _h.SVD(out _u, out _e, out _v);
            _r = _v * _u.Transpose();        
        
            if (_r.Determinant<0) 
            { 
                _v.V02 = (-_v.V02);
                _v.V12 = (-_v.V12);
                _v.V22 = (-_v.V22);
                _r = _v * _u.Transpose();
                Debug.LogWarning("Reflection case");
            }
        
            _translation = NegativeMatrix(_r) * _centroidA + _centroidB;
            _transformationMatrix = AccordToUnityMatrix(_transformationMatrix, _r, _translation);
            _transformationMatrix.SetTRS(AccordtoUnity(_translation), Quaternion.LookRotation(_transformationMatrix.GetColumn(1),
                _transformationMatrix.GetColumn(2)), UnityEngine.Vector3.one);
        
            _triad.Anchor.transform.position = _transformationMatrix.MultiplyPoint(_initPosition);
            _triad.Anchor.transform.rotation = Quaternion.LookRotation(_transformationMatrix.GetColumn(1), _transformationMatrix.GetColumn(2))* _initRotation;
        }
    
        private Matrix3x3 CovarianceMatrixStep( Accord.Math.Vector3 difSetA, Accord.Math.Vector3 difSetB )
        {
            Matrix3x3 M;
            M.V00 = difSetA.X * difSetB.X;
            M.V01 = difSetA.X * difSetB.Y;
            M.V02 = difSetA.X * difSetB.Z;

            M.V10 = difSetA.Y * difSetB.X;
            M.V11 = difSetA.Y * difSetB.Y;
            M.V12 = difSetA.Y * difSetB.Z;

            M.V20 = difSetA.Z * difSetB.X;
            M.V21 = difSetA.Z * difSetB.Y;
            M.V22 = difSetA.Z * difSetB.Z;

            return M;
        }
    
        private Accord.Math.Vector3 UnitytoAccord( UnityEngine.Vector3 pos )
        {
            Accord.Math.Vector3 posTransformed = new Accord.Math.Vector3();
            posTransformed.X = pos.x;
            posTransformed.Y = pos.y;
            posTransformed.Z = pos.z;

            return posTransformed;
        }
    
        private UnityEngine.Vector3 AccordtoUnity(Accord.Math.Vector3 pos)
        {
            UnityEngine.Vector3 posTransformed = new UnityEngine.Vector3();
            posTransformed.x = pos.X;
            posTransformed.y = pos.Y;
            posTransformed.z = pos.Z;

            return posTransformed;
        }
    
        private Matrix3x3 ZeroMatrix(Matrix3x3 m)
        {
            m.V00 = 0;
            m.V01 = 0;
            m.V02 = 0;
            m.V10 = 0;
            m.V11 = 0;
            m.V12 = 0;
            m.V20 = 0;
            m.V21 = 0;
            m.V22 = 0;

            return m;
        }
    
        private Matrix3x3 NegativeMatrix(Matrix3x3 m)
        {
            m.V00 *= (-1);
            m.V01 *= (-1);
            m.V02 *= (-1);
            m.V10 *= (-1);
            m.V11 *= (-1);
            m.V12 *= (-1);
            m.V20 *= (-1);
            m.V21 *= (-1);
            m.V22 *= (-1);

            return m;
        }
    
        private UnityEngine.Matrix4x4 AccordToUnityMatrix(UnityEngine.Matrix4x4 UnityM, Accord.Math.Matrix3x3 RotationM, Accord.Math.Vector3 Trans)
        {
            UnityM.m00 = RotationM.V00;
            UnityM.m10 = RotationM.V10;
            UnityM.m20 = RotationM.V20;
        
            UnityM.m01 = RotationM.V01;
            UnityM.m11 = RotationM.V11;
            UnityM.m21 = RotationM.V21;
        
            UnityM.m02 = RotationM.V02;
            UnityM.m12 = RotationM.V12;
            UnityM.m22 = RotationM.V22;

            UnityM.m03 = Trans.X;
            UnityM.m13 = Trans.Y;
            UnityM.m23 = Trans.Z;

            UnityM.m30 = 0;
            UnityM.m31 = 0;
            UnityM.m32 = 0;
            UnityM.m33 = 1;

            return UnityM;
        }
    }
}
