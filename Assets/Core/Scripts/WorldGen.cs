using UnityEngine;

[ExecuteInEditMode]
public class WorldGen : MonoBehaviour
{
    public float scale = 0.1f;
    public int width = 256, height = 256;
    public NoiseValues[] noiseValues;

    private GameObject plane;
    private Material outputMat;
    private Texture2D outputTex;

    [System.Serializable]
    public class NoiseValues
    {
        public FastNoise.NoiseType noiseType = FastNoise.NoiseType.Simplex;
        public int seed = 1337;
        public float frequency = 0.01f;
        public float gradientPerturbAmp = 1;
        public FastNoise.Interp interp = FastNoise.Interp.Quintic;

        public int cellularDistanceIndex0 = 0;
        public int cellularDistanceIndex1 = 1;
        public FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean;
        public float cellularJitter = 0.45f;
        public FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.CellValue;

        public float fractalGain = 0.5f;
        public float fractalLacunarity = 2;
        public int fractalOctaves = 3;
        public FastNoise.FractalType fractalType = FastNoise.FractalType.FBM;
        
        public void ApplyValuesTo(FastNoise instance)
        {
            instance.SetNoiseType(noiseType);
            instance.SetSeed(seed);
            instance.SetFrequency(frequency);
            instance.SetGradientPerturbAmp(gradientPerturbAmp);
            instance.SetInterp(interp);

            instance.SetCellularDistance2Indicies(cellularDistanceIndex0, cellularDistanceIndex1);
            instance.SetCellularDistanceFunction(cellularDistanceFunction);
            instance.SetCellularJitter(cellularJitter);
            instance.SetCellularReturnType(cellularReturnType);

            instance.SetFractalGain(fractalGain);
            instance.SetFractalLacunarity(fractalLacunarity);
            instance.SetFractalOctaves(fractalOctaves);
            instance.SetFractalType(fractalType);
        }
        public void ReadValuesFrom(FastNoise instance)
        {
            noiseType = instance.GetNoiseType();
            seed = instance.GetSeed();
            frequency = instance.GetFrequency();
            gradientPerturbAmp = instance.GetGradientPerturbAmp();
            interp = instance.GetInterp();

            cellularDistanceIndex0 = instance.GetCellularDistanceIndex0();
            cellularDistanceIndex1 = instance.GetCellularDistanceIndex1();
            cellularDistanceFunction = instance.GetCellularDistanceFunction();
            cellularJitter = instance.GetCellularJitter();
            cellularReturnType = instance.GetCellularReturnType();

            fractalGain = instance.GetFractalGain();
            fractalLacunarity = instance.GetFractalLacunarity();
            fractalOctaves = instance.GetFractalOctaves();
            fractalType = instance.GetFractalType();
        }
    }

    void OnEnable()
    {
        Generate();
    }
    void OnDisable()
    {
        DestroyOutput();
    }
    void OnValidate()
    {
        Generate();
    }

    private void PrepareOutput()
    {
        if (outputTex == null || width != outputTex.width || height != outputTex.height)
        {
            if (outputTex != null)
                DestroyImmediate(outputTex);
            outputTex = new Texture2D(width, height);
        }
        if (outputMat == null)
            outputMat = new Material(Shader.Find("Unlit/Texture"));

        if (plane == null)
        {
            plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = Vector3.up * 5;
            plane.GetComponent<Renderer>().sharedMaterial = outputMat;
        }
    }
    private void DestroyOutput()
    {
        DestroyImmediate(outputTex);
        outputTex = null;
        DestroyImmediate(outputMat);
        outputMat = null;
        DestroyImmediate(plane);
        plane = null;
    }
    private void Generate()
    {
        PrepareOutput();

        var fn = new FastNoise();
        int pixelCount = width * height;
        Color[] pixels = new Color[pixelCount];
        for (int i = 0; i < pixelCount; i++)
        {
            float value = 1;
            for (int j = 0; j < noiseValues.Length; j++)
            {
                noiseValues[j].ApplyValuesTo(fn);
                int col = i % width;
                int row = i / width;
                value *= (fn.GetNoise(col * scale, row * scale) + 1) / 2;
            }
            pixels[i] = new Color(value, value, value, 1);
        }
        outputTex.SetPixels(pixels);
        outputTex.Apply();
        outputMat.SetTexture("_MainTex", outputTex);
    }
}
