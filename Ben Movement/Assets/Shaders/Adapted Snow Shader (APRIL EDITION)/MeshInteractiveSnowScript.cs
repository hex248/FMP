using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshInteractiveSnowScript : MonoBehaviour
{
    public Shader _snowHeightMapUpdate;
    public Texture _stepPrint;
    public Material _snowMaterial;
    public List<TrailDrawer> trailDrawers;
    public List<Transform> _trailsPositions;
    public List<float> _trailSizes = new List<float>();
    public List<Texture2D> _trailBrushes = new List<Texture2D>();
    public List<float> _trailDistance;

    //public float _drawDistance = 0.3f;

    Material _heightMapUpdate;
    public CustomRenderTexture _snowHeightMap;
    CustomRenderTexture _prevHeightMap;

    Renderer mesh;

    public int _index = 0;

    private readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
    private readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
    private readonly int PreviousTexture = Shader.PropertyToID("_PreviousTexture");
    private readonly int HeightMap = Shader.PropertyToID("_HeightMap");
    private readonly int Offset = Shader.PropertyToID("_Offset");
    private readonly int Brush = Shader.PropertyToID("_DrawBrush");

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        DrawTrails();
        UpdateSnowMaterial();
        _snowHeightMap.Update();
    }

    private void Initialize()
    {
        var material = new Material(_snowMaterial);

        _heightMapUpdate = CreateHeightMapUpdate(_snowHeightMapUpdate);
        _snowHeightMap = CreateHeightMap(1024, 1024, _heightMapUpdate);

        mesh = gameObject.GetComponent<Renderer>();
        mesh.material = material;
        mesh.material.SetTexture(HeightMap, _snowHeightMap);

        _snowHeightMap.Initialize();
    }

    void UpdateSnowMaterial()
    {
        // create array of tessellation points based on trails being drawn
        Vector4[] posArray = new Vector4[100];
        var tessellationPoints = trailDrawers.FindAll(e => e.enabled);
        for (int i = 0; i < tessellationPoints.Count; i++)
        {
            posArray[i] = tessellationPoints[i].drawTransform.position;
        }
        mesh.material.SetVectorArray("_DrawPositions", posArray);
        mesh.material.SetInt("_DrawPositionNum", tessellationPoints.Count);

        // set the properties of the copied material based on snow asset
        mesh.material.SetFloat("_Tess", _snowMaterial.GetFloat("_Tess"));
        mesh.material.SetFloat("_MinTessDistance", _snowMaterial.GetFloat("_MinTessDistance") + Time.deltaTime);
        mesh.material.SetFloat("_MaxTessDistance", _snowMaterial.GetFloat("_MaxTessDistance"));
        mesh.material.SetInt("_ShadingDetail", _snowMaterial.GetInt("_ShadingDetail"));
    }

    private void DrawTrails()
    {

        if (_trailsPositions.Count < 1) return;

        if (_index >= _trailsPositions.Count)
            _index = 0;
        var trail = _trailsPositions[_index];

        Ray ray = new Ray(trail.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, _trailDistance[_index]))
        {
            if (hit.collider.name == gameObject.name)
            {
                Vector2 hitTextureCoord = hit.textureCoord;
                float angle = trail.transform.rotation.eulerAngles.y; // texture rotation angle

                _heightMapUpdate.SetVector(DrawPosition, hitTextureCoord);
                _heightMapUpdate.SetTexture(PreviousTexture, _prevHeightMap);
                _heightMapUpdate.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
                _heightMapUpdate.SetFloat(Offset, _trailSizes[_index] / 100);
                _heightMapUpdate.SetTexture(Brush, _trailBrushes[_index]);
                _heightMapUpdate.SetFloat("_DeltaTime", Time.deltaTime);
            }
        }
        _prevHeightMap = _snowHeightMap;

        _index++;

        if (_index >= _trailsPositions.Count)
            _index = 0;
    }

    private CustomRenderTexture CreateHeightMap(int width, int height, Material material)
    {
        CustomRenderTexture texture = new CustomRenderTexture(width, height);

        texture.material = material;
        texture.updateMode = CustomRenderTextureUpdateMode.Realtime;
        texture.doubleBuffered = true;
        texture.initializationMode = CustomRenderTextureUpdateMode.Realtime;
        texture.initializationSource = CustomRenderTextureInitializationSource.Material;
        texture.initializationMaterial = material;

        return texture;
    }

    private Material CreateHeightMapUpdate(Shader shader)
    {
        var material = new Material(shader);
        material.SetVector(DrawPosition, new Vector4(-1, -1, 0, 0));
        return material;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<TrailDrawer>() != null)
        {
            var trailDrawer = other.GetComponent<TrailDrawer>();
            if (trailDrawer.objectType == ObjectType.Player)
            {
                if (!_trailsPositions.Contains(trailDrawer.drawTransform))
                {
                    trailDrawers.Add(trailDrawer);
                    _trailsPositions.Add(trailDrawer.drawTransform);
                    _trailSizes.Add(trailDrawer.drawSize);
                    _trailBrushes.Add(trailDrawer.drawBrush);
                    _trailDistance.Add(trailDrawer.drawDistance);
                    trailDrawer.index = _trailsPositions.Count - 1;
                }
                else
                {
                    trailDrawer.index = trailDrawers.IndexOf(trailDrawer);
                    trailDrawers[trailDrawer.index] = trailDrawer;
                    _trailSizes[trailDrawer.index] = trailDrawer.drawSize;
                    _trailBrushes[trailDrawer.index] = trailDrawer.drawBrush;
                    _trailDistance[trailDrawer.index] = trailDrawer.drawDistance;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<TrailDrawer>() != null)
        {
            var trailDrawer = other.GetComponent<TrailDrawer>();
            if (_trailsPositions.Contains(trailDrawer.drawTransform))
            {
                trailDrawers.RemoveAt(trailDrawer.index);
                _trailsPositions.RemoveAt(trailDrawer.index);
                _trailBrushes.RemoveAt(trailDrawer.index);
                _trailSizes.RemoveAt(trailDrawer.index);
                _trailDistance.RemoveAt(trailDrawer.index);
            }
        }
    }

    public void RemoveObject(int removeIDX)
    {
        StartCoroutine(RemoveObjectCoroutine(removeIDX));
    }

    IEnumerator RemoveObjectCoroutine(int removeIDX)
    {
        yield return new WaitForEndOfFrame();
        trailDrawers.RemoveAt(removeIDX);
        _trailsPositions.RemoveAt(removeIDX);
        _trailBrushes.RemoveAt(removeIDX);
        _trailSizes.RemoveAt(removeIDX);
        _trailDistance.RemoveAt(removeIDX);
    }
}
