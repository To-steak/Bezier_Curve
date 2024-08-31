using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

public class Evaluation : MonoBehaviour
{
    public GameObject cube;
    public GameObject vertex;
    
    private Vector4 basisU;
    private Vector4 basisV;
    private Vector4 dBasisU;
    private Vector4 dBasisV;
    private Vector3 p;
    private Vector3 dpdu;
    private Vector3 dpdv;
    [HideInInspector]
    public Vector3[] bezpatch = new Vector3[]
    {
        new Vector3(-10.0f, -10.0f, +15.0f),
        new Vector3(-5.0f,  0.0f, +15.0f),
        new Vector3(+5.0f,  0.0f, +15.0f),
        new Vector3(+10.0f, 0.0f, +15.0f),

        // Row 1
        new Vector3(-15.0f, 0.0f, +5.0f),
        new Vector3(-5.0f,  0.0f, +5.0f),
        new Vector3(+5.0f,  20.0f, +5.0f),
        new Vector3(+15.0f, 0.0f, +5.0f),

        // Row 2
        new Vector3(-15.0f, 0.0f, -5.0f),
        new Vector3(-5.0f,  0.0f, -5.0f),
        new Vector3(+5.0f,  0.0f, -5.0f),
        new Vector3(+15.0f, 0.0f, -5.0f),

        // Row 3
        new Vector3(-10.0f, 10.0f, -15.0f),
        new Vector3(-5.0f,  0.0f, -15.0f),
        new Vector3(+5.0f,  0.0f, -15.0f),
        new Vector3(+25.0f, 10.0f, -15.0f)
    };

    [Range(0, 1)]
    public float u;

    [Range(0, 1)] public float v;
    
    // Start is called before the first frame update
    private void Start()
    {
        basisU = BernsteinBasis(u);
        basisV = BernsteinBasis(v);

        p = CubicBezierSum(bezpatch, basisU, basisV);

        dBasisU = DerivativeBernsteinBasis(u);
        dBasisV = DerivativeBernsteinBasis(v);

        dpdu = CubicBezierSum(bezpatch, dBasisU, basisU);
        dpdv = CubicBezierSum(bezpatch, dBasisV, basisV);
        
        Debug.Log($"basisU: {basisU}\n" +
                  $"basisV: {basisV}\n" +
                  $"p: {p}\n" +
                  $"dBasisU: {dBasisU}\n" +
                  $"dBasisV: {dBasisV}\n" +
                  $"dpdu: {dpdu}\n" +
                  $"dpdv: {dpdv}");
        
        Generate(cube, bezpatch);
        Generate(vertex, p);
    }

    public Vector4 BernsteinBasis(float t)
    {
        float invT = 1.0f - t;
        
        return new Vector4(
            invT * invT * invT,
            3.0f * t * invT * invT,
            3.0f * t * t * invT,
            t * t * t);
    }

    public Vector4 DerivativeBernsteinBasis(float t)
    {
        float invT = 1.0f - t;

        return new Vector4(
            -3 * invT * invT,
            3 * invT * invT - 6 * t * invT, 
            6 * t * invT - 3 * t * t, 
            3 * t * t);
    }
    
    public Vector3 CubicBezierSum(Vector3[] patch, Vector4 u, Vector4 v)
    {
        Vector3 sum = Vector3.zero;
        sum = v.x * (u.x * patch[0] +
                          u.y * patch[1] +
                          u.z * patch[2] +
                          u.w * patch[3]);
        
        sum += v.y * (u.x * patch[4] +
                          u.y * patch[5] +
                          u.z * patch[6] +
                          u.w * patch[7]);
        
        sum += v.z * (u.x * patch[8] +
                           u.y * patch[9] +
                           u.z * patch[10] +
                           u.w * patch[11]);
        
        sum += v.w * (u.x * patch[12] +
                           u.y * patch[13] +
                           u.z * patch[14] +
                           u.w * patch[15]);
        return sum;
    }
    
    public Func<Vector4, Vector3> PosL = vector => new Vector3(vector.x, vector.y, vector.z);

    private void Generate(GameObject prefab, params Vector3[] pos)
    {
        for (int i = 0; i < pos.Length; i++)
        {
            Instantiate(prefab, pos[i], Quaternion.identity);
        }
    }
}

[CustomEditor(typeof(Evaluation))]
public class EvaluationEditor : Editor
{
    private void OnSceneGUI()
    {
        Evaluation evaluation = (Evaluation)target;

        float lineWidth = 2f;
        Color lineColor = Color.blue;

        // 첫 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[0], 
            evaluation.bezpatch[3], 
            evaluation.bezpatch[1], 
            evaluation.bezpatch[2], 
            lineColor, 
            null, 
            lineWidth);

        // 두 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[4], 
            evaluation.bezpatch[7], 
            evaluation.bezpatch[5], 
            evaluation.bezpatch[6], 
            lineColor, 
            null, 
            lineWidth);

        // 세 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[8], 
            evaluation.bezpatch[11], 
            evaluation.bezpatch[9], 
            evaluation.bezpatch[10], 
            lineColor, 
            null, 
            lineWidth);

        // 네 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[12], 
            evaluation.bezpatch[15], 
            evaluation.bezpatch[13], 
            evaluation.bezpatch[14], 
            lineColor, 
            null, 
            lineWidth);

        // 다섯 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[0], 
            evaluation.bezpatch[12], 
            evaluation.bezpatch[4], 
            evaluation.bezpatch[8], 
            lineColor, 
            null, 
            lineWidth);

        // 여섯 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[1], 
            evaluation.bezpatch[13], 
            evaluation.bezpatch[5], 
            evaluation.bezpatch[9], 
            lineColor, 
            null, 
            lineWidth);

        // 일곱 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[2], 
            evaluation.bezpatch[14], 
            evaluation.bezpatch[6], 
            evaluation.bezpatch[10], 
            lineColor, 
            null, 
            lineWidth);

        // 여덟 번째 비즈 곡선
        Handles.DrawBezier(
            evaluation.bezpatch[3], 
            evaluation.bezpatch[15], 
            evaluation.bezpatch[7], 
            evaluation.bezpatch[11], 
            lineColor, 
            null, 
            lineWidth);
    }
}