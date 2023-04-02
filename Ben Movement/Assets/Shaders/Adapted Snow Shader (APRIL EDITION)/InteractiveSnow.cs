using UnityEngine;
using UnityEngine.Rendering;

public class InteractiveSnow : MonoBehaviour
{
    public Shader _snowHeightMapUpdate;
    public Texture _stepPrint;
    public Material _snowMaterial;
    public Transform[] _trailsPositions;

    public float _drawDistance = 0.3f;
    public float _offset = 0.2f;

    Material _heightMapUpdate;
    public CustomRenderTexture _snowHeightMap;
    public CustomRenderTexture _prevHeightMap;
    MeshRenderer meshRenderer;

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
        _snowHeightMap = CreateHeightMap(512, 512, _heightMapUpdate);

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = material;
        meshRenderer.material.SetTexture(HeightMap, _snowHeightMap);

        _snowHeightMap.Initialize();
    }

    void UpdateSnowMaterial()
    {
        // set the properties of the copied material based on snow asset
        meshRenderer.material.SetFloat("_Tess", _snowMaterial.GetFloat("_Tess"));
        meshRenderer.material.SetFloat("_MinTessDistance", _snowMaterial.GetFloat("_MinTessDistance") + Time.deltaTime);
        meshRenderer.material.SetFloat("_MaxTessDistance", _snowMaterial.GetFloat("_MaxTessDistance"));
        meshRenderer.material.SetInt("_ShadingDetail", _snowMaterial.GetInt("_ShadingDetail"));

        // convert array of transforms to array of positions (V4)
        Vector4[] posArray = new Vector4[_trailsPositions.Length];
        for (int i = 0; i < _trailsPositions.Length; i++)
        {
            posArray[i] = _trailsPositions[i].position;
        }
        meshRenderer.material.SetVectorArray("_DrawPositions", posArray);
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
