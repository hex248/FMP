using UnityEngine;
using UnityEngine.Rendering;

public class TerrainInteractiveSnow : MonoBehaviour
{
    public Shader _snowHeightMapUpdate;
    public Texture _stepPrint;
    public Material _snowMaterial;
    public Transform[] _trailsPositions;

    public float _drawDistance = 0.3f;
    public float _offset = 0.2f;

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
        // set the properties of the copied material based on snow asset
        terrain.materialTemplate.SetFloat("_Tess", _snowMaterial.GetFloat("_Tess"));
        terrain.materialTemplate.SetFloat("_MinTessDistance", _snowMaterial.GetFloat("_MinTessDistance") + Time.deltaTime);
        terrain.materialTemplate.SetFloat("_MaxTessDistance", _snowMaterial.GetFloat("_MaxTessDistance"));
        terrain.materialTemplate.SetInt("_ShadingDetail", _snowMaterial.GetInt("_ShadingDetail"));

        // convert array of transforms to array of positions (V4)
        Vector4[] posArray = new Vector4[_trailsPositions.Length];
        for (int i = 0; i < _trailsPositions.Length; i++)
        {
            posArray[i] = _trailsPositions[i].position;
        }
        terrain.materialTemplate.SetVectorArray("_DrawPositions", posArray);
    }

    private void DrawTrails()
    {
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
                _heightMapUpdate.SetFloat(Offset, _offset);
                _heightMapUpdate.SetFloat("_DeltaTime", Time.deltaTime);
            }
        }
        _prevHeightMap = _snowHeightMap;

        _index++;

        if (_index >= _trailsPositions.Length)
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
}
