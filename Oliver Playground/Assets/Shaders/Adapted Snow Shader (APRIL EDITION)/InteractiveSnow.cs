using UnityEngine;
using UnityEngine.Rendering;

public class InteractiveSnow : MonoBehaviour
{
    public Renderer heightMapVisualiser;

    public Shader _snowHeightMapUpdate;
    public Texture _stepPrint;
    public Material _snowMaterial;
    public Transform[] _trailsPositions;

    public float _drawDistance = 0.3f;

    public Material _heightMapUpdate;
    public CustomRenderTexture _snowHeightMap;
    public CustomRenderTexture _prevHeightMap;

    int _index = 0;

    private readonly int DrawPosition = Shader.PropertyToID("_DrawPosition");
    private readonly int DrawAngle = Shader.PropertyToID("_DrawAngle");
    private readonly int DrawBrush = Shader.PropertyToID("_DrawBrush");
    private readonly int PreviousTexture = Shader.PropertyToID("_PreviousTexture");
    private readonly int HeightMap = Shader.PropertyToID("_HeightMap");

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        DrawTrails();
        _snowHeightMap.Update();
    }

    private void Initialize()
    {
        var material = new Material(_snowMaterial);

        _heightMapUpdate = CreateHeightMapUpdate(_snowHeightMapUpdate, _stepPrint);
        _snowHeightMap = CreateHeightMap(512, 512, _heightMapUpdate);
        _prevHeightMap = _snowHeightMap;

        var terrain = gameObject.GetComponent<Terrain>();
        terrain.materialTemplate = material;
        terrain.materialTemplate.SetTexture(HeightMap, _snowHeightMap);
        //heightMapVisualiser.material = _heightMapUpdate;
        heightMapVisualiser.material.mainTexture = _snowHeightMap;

        _snowHeightMap.Initialize();
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
                _heightMapUpdate.SetFloat(DrawAngle, angle * Mathf.Deg2Rad);
            }
        }

        _index++;

        if (_index >= _trailsPositions.Length)
            _index = 0;
    }

    private CustomRenderTexture CreateHeightMap(int weight, int height, Material material)
    {
        var texture = new CustomRenderTexture(weight, height);

        texture.dimension = TextureDimension.Tex2D;
        texture.format = RenderTextureFormat.R8;
        texture.material = material;
        texture.updateMode = CustomRenderTextureUpdateMode.Realtime;
        texture.doubleBuffered = true;

        return texture;
    }

    private Material CreateHeightMapUpdate(Shader shader, Texture stepPrint)
    {
        var material = new Material(shader);
        material.SetTexture(DrawBrush, stepPrint);
        material.SetTexture(PreviousTexture, _prevHeightMap);
        material.SetVector(DrawPosition, new Vector4(-1, -1, 0, 0));
        return material;
    }
}
