using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainInteractiveSnow : MonoBehaviour
{
    public Shader _snowHeightMapUpdate;
    public Texture _stepPrint;
    public Material _snowMaterial;
    public List<Transform> _trailsPositions;
    public List<float> _trailSizes;

    public float _drawDistance = 0.3f;

    Material _heightMapUpdate;
    public CustomRenderTexture _snowHeightMap;
    CustomRenderTexture _prevHeightMap;

    Terrain terrain;

    int _index = 0;

    private readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
    private readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
    private readonly int DrawBrush = Shader.PropertyToID("_DrawBrush");
    private readonly int PreviousTexture = Shader.PropertyToID("_PreviousTexture");
    private readonly int HeightMap = Shader.PropertyToID("_HeightMap");
    private readonly int Offset = Shader.PropertyToID("_Offset");

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

        _heightMapUpdate = CreateHeightMapUpdate(_snowHeightMapUpdate, _stepPrint);
        _snowHeightMap = CreateHeightMap(1024, 1024, _heightMapUpdate);

        terrain = gameObject.GetComponent<Terrain>();
        terrain.materialTemplate = material;
        terrain.materialTemplate.SetTexture(HeightMap, _snowHeightMap);

        _snowHeightMap.Initialize();
    }

    void UpdateSnowMaterial()
    {
        // convert array of transforms to array of positions (V4)
        Vector4[] posArray = new Vector4[100];
        for (int i = 0; i < _trailsPositions.Count; i++)
        {
            posArray[i] = _trailsPositions[i].position;
        }
        terrain.materialTemplate.SetVectorArray("_DrawPositions", posArray);

        // set positions after (triggers an update of positions)

        // set the properties of the copied material based on snow asset
        terrain.materialTemplate.SetFloat("_Tess", _snowMaterial.GetFloat("_Tess"));
        terrain.materialTemplate.SetFloat("_MinTessDistance", _snowMaterial.GetFloat("_MinTessDistance") + Time.deltaTime);
        terrain.materialTemplate.SetFloat("_MaxTessDistance", _snowMaterial.GetFloat("_MaxTessDistance"));
        terrain.materialTemplate.SetInt("_ShadingDetail", _snowMaterial.GetInt("_ShadingDetail"));
    }

    private void DrawTrails()
    {
        if (_trailsPositions.Count < 1) return;
        var trail = _trailsPositions[_index];

        Ray ray = new Ray(trail.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, _drawDistance))
        {
            if (hit.collider.name == gameObject.name)
            {
                Vector2 hitTextureCoord = hit.textureCoord;
                float angle = trail.transform.rotation.eulerAngles.y; // texture rotation angle

                _heightMapUpdate.SetVector(DrawPosition, hitTextureCoord);
                _heightMapUpdate.SetTexture(PreviousTexture, _prevHeightMap);
                _heightMapUpdate.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
                _heightMapUpdate.SetFloat(Offset, _trailSizes[_index] / 100);
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

    private Material CreateHeightMapUpdate(Shader shader, Texture stepPrint)
    {
        var material = new Material(shader);
        material.SetTexture(DrawBrush, stepPrint);
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
                    _trailsPositions.Add(trailDrawer.drawTransform);
                    _trailSizes.Add(trailDrawer.drawSize);

                    terrain.materialTemplate.SetInt("_DrawPositionNum", _trailsPositions.Count);
                }
                else
                {
                    // update draw size

                    _trailSizes[_trailsPositions.IndexOf(trailDrawer.drawTransform)] = trailDrawer.drawSize;
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
                _trailSizes.RemoveAt(_trailsPositions.IndexOf(trailDrawer.drawTransform));
                _trailsPositions.Remove(trailDrawer.drawTransform);
            }
        }
    }

    public void RemoveObject(GameObject removedObject)
    {
        int removedIDX = _trailsPositions.IndexOf(removedObject.transform);
        _trailsPositions.RemoveAt(removedIDX);
        _trailSizes.RemoveAt(removedIDX);
    }
}
