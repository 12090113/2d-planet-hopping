using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CircularShadow : MonoBehaviour
{

    static readonly FieldInfo _meshField;
    static readonly FieldInfo _shapePathField;
    static readonly MethodInfo _generateShadowMeshMethod;

    ShadowCaster2D _shadowCaster;

    public int points = 8;
    public float radius = 0.5f;

    static CircularShadow()
    {
        _meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

        _generateShadowMeshMethod = typeof(ShadowCaster2D)
                                    .Assembly
                                    .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
    }

    private void Start()
    {
        _shadowCaster = GetComponent<ShadowCaster2D>();
        UpdateShadow();
    }

    public void UpdateShadow()
    {
        Vector3[] point = new Vector3[points];
        float angle = 2 * Mathf.PI / points;
        for (int i = 0; i < points; i++)
        {
            point[i].x = radius * Mathf.Sin(i * angle);
            point[i].y = radius * Mathf.Cos(i * angle);
        }

        _shapePathField.SetValue(_shadowCaster, point);
        _meshField.SetValue(_shadowCaster, new Mesh());
        _generateShadowMeshMethod.Invoke(_shadowCaster, new object[] { _meshField.GetValue(_shadowCaster), _shapePathField.GetValue(_shadowCaster) });
    }
}