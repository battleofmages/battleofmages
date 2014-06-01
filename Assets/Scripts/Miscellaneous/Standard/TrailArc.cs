using UnityEngine;
using System.Collections;
 
public class TrailArc : MonoBehaviour 
{
 
	int savedIndex;
	int pointIndex;
 
    // Material - Particle Shader with "Tint Color" property
    public Material material;
 
    // Emit
    public bool emit
    {
        get { return Emit; }
        set { Emit = value; }
    }
    bool Emit = true;
    bool emittingDone = false;
 
	//Minimum velocity (script terminates)
	public float minVel = 10;
 
    // Facing
    public bool faceCamera = true;
 
    // Lifetime of each segment
    public float lifetime = 1;
    float lifeTimeRatio = 1;
    float fadeOutRatio;
 
    // Colors
    public Color[] colors;
 
    // Widths
    public float[] widths;
 
    // Optimization
    public float pointDistance = 0.5f;
    float pointSqrDistance = 0;
    public int segmentsPerPoint = 4;
    float tRatio;
 
    // Print Output
    public bool printResults = false;
    public bool printSavedPoints = false;
    public bool printSegmentPoints = false;
 
    // Objects
    GameObject trail = null;
    Mesh mesh = null;
    Material trailMaterial = null;
 
    // Points
    Vector3[] saved;
    Vector3[] savedUp;
    int savedCnt = 0;
    Vector3[] points;
    Vector3[] pointsUp;
    int pointCnt = 0;
 
    // Segment Appearance Normalization
    int displayCnt = 0;
    float lastPointCreationTime = 0;
    float averageCreationTime = 0;
    float averageInsertionTime = 0;
    float elapsedInsertionTime = 0;
 
    // Initialization
    bool initialized = false;
 
    void Start ()
    {
	if(gameObject.rigidbody.velocity.magnitude < minVel)
			Destroy(this);
 
        // Data Inititialization
        saved = new Vector3[60];
        savedUp = new Vector3[saved.Length];
        points = new Vector3[saved.Length * segmentsPerPoint];
        pointsUp = new Vector3[points.Length];
        tRatio = 1f / (segmentsPerPoint);
        pointSqrDistance = pointDistance * pointDistance;
 
        // Create the mesh object
        trail = new GameObject("Trail");
        trail.transform.position = Cache.vector3Zero;
        trail.transform.rotation = Cache.quaternionIdentity;
        trail.transform.localScale = Vector3.one;
        MeshFilter meshFilter = (MeshFilter) trail.AddComponent(typeof(MeshFilter));
        mesh = meshFilter.mesh;
        trail.AddComponent(typeof(MeshRenderer));
        trailMaterial = new Material(material);
        fadeOutRatio = trailMaterial.GetColor("_TintColor").a;
        trail.renderer.material = trailMaterial;
    }
 
    void printPoints()
    {
        if(savedCnt == 0)
            return;
        string s = "Saved Points at time " + Time.time + ":\n";
        for(int i = 0; i < savedCnt; i++)
            s += "Index: " + i + "\tPos: " + saved[i] + "\n";
        print(s);
    }
 
    void printAllPoints()
    {
        if(pointCnt == 0)
            return;
        string s = "Points at time " + Time.time + ":\n";
        for(int i = 0; i < pointCnt; i++)
            s += "Index: " + i + "\tPos: " + points[i] + "\n";
        print(s);
    }
 
    void findCoordinates(int index)
    {
        if(index == 0 || index >= savedCnt-2)
            return;
        Vector3 P0 = saved[index-1];
        Vector3 P1 = saved[index];
        Vector3 P2 = saved[index+1];
        Vector3 P3 = saved[index+2];
        Vector3 T1 = 0.5f * (P2 - P0);
        Vector3 T2 = 0.5f * (P3 - P1);
        int pointIndex = index * segmentsPerPoint;
        for(int i = pointIndex; i < pointIndex+segmentsPerPoint; i++)
        {
            float t = (i-pointIndex) * tRatio;
            float t2 = t*t;
            float t3 = t2*t;
            float blend1 = 2*t3 - 3*t2 + 1;
            float blend2 = 3*t2 - 2*t3;
            float blend3 = t3 - 2*t2  + t;
            float blend4 = t3 - t2;
            int pntInd = i - segmentsPerPoint;
            points[pntInd] = blend1*P1 + blend2*P2 + blend3*T1 + blend4*T2;
            pointsUp[pntInd] = Vector3.Lerp(savedUp[index], savedUp[index+1], t);
        }
        pointCnt = pointIndex;
    }
 
    void Update ()
    {
        //try
        //{
            Vector3 position = transform.position;
            // Wait till the object is active (update called) and emitting
            if( ! initialized && Emit)
            {
                // Place the first point behind this as a starter projected point
                saved[savedCnt] = transform.TransformPoint(0,0,-pointDistance);
                savedUp[savedCnt] = transform.up;
                savedCnt++;
                // Place the second point at the current position
                saved[savedCnt] = position;
                savedUp[savedCnt] = transform.up;
                savedCnt++;
                // Begin tracking the saved point creation time
                lastPointCreationTime = Time.time;
                initialized = true;
            }
 
            if(printSavedPoints)
                printPoints();
            if(printSegmentPoints)
                printAllPoints();
 
            // Emitting - Designed for one-time use
            if( ! Emit )
            {
                if( ! emittingDone && pointCnt > 0 )
                {
                    // Save two final points projected from the ending point
                    saved[savedCnt] = transform.TransformPoint(0,0,pointDistance);
                    savedUp[savedCnt] = transform.up;
                    savedCnt++;
                    findCoordinates(savedCnt-3);
                    // This makes the trail fill the actual entire path
                    saved[savedCnt] = transform.TransformPoint(0,0,pointDistance*2);
                    savedUp[savedCnt] = transform.up;
                    savedCnt++;
                    findCoordinates(savedCnt-3);
                }
                emittingDone = true;
            }
            if(emittingDone)
                Emit = false;
 
            if(Emit)
            {
 
                // Do we save a new point?
                if( (saved[savedCnt-1] - position).sqrMagnitude > pointSqrDistance)
                {
                    saved[savedCnt] = position;
                    savedUp[savedCnt] = transform.up;
                    savedCnt++;
 
                    // Calc the average point display time
                    if(averageCreationTime == 0)
                        averageCreationTime = Time.time - lastPointCreationTime;
                    else
                    {
                        float elapsedTime = Time.time - lastPointCreationTime;
                        averageCreationTime = (averageCreationTime + elapsedTime) * 0.5f;
                    }
                    averageInsertionTime = averageCreationTime * tRatio;
                    lastPointCreationTime = Time.time;
 
                    // Calc the last saved segment coordinates
                    if(savedCnt > 3)
                        findCoordinates(savedCnt-3);
                }
            }
 
            // Do we fade it out?
            if( ! Emit && displayCnt == pointCnt)
            {
                Color color = trailMaterial.GetColor("_TintColor");
                color.a -= fadeOutRatio * lifeTimeRatio * Time.deltaTime;
                if(color.a > 0)
                    trailMaterial.SetColor("_TintColor", color);
                else
                {
                    if(printResults)
                        print("Trail effect ending with a segment count of: " + pointCnt);
                    Destroy(trail);
                    Destroy(gameObject);
                }
                return;
            }
 
            // Do we display any new points?
            if(displayCnt < pointCnt)
            {
                elapsedInsertionTime += Time.deltaTime;
                while(elapsedInsertionTime > averageInsertionTime)
                {
                    if(displayCnt < pointCnt)
                        displayCnt++;
                    elapsedInsertionTime -= averageInsertionTime;
                }
            }
 
            // Do we render this?
            if(displayCnt < 2)
            {
                trail.renderer.enabled = false;
                return;
            }
            trail.renderer.enabled = true;
 
            // Common data
            lifeTimeRatio = 1f / lifetime;
            Color[] meshColors;
 
            // Rebuild the mesh
            Vector3[] vertices = new Vector3[displayCnt * 2];
            Vector2[] uvs = new Vector2[displayCnt * 2];
            int[] triangles = new int[(displayCnt-1) * 6];
            meshColors = new Color[displayCnt * 2];
 
            float pointRatio = 1f / (displayCnt-1);
            Vector3 cameraPos = Camera.main.transform.position;
            for(int i = 0; i < displayCnt; i++)
            {
                Vector3 point = points[i];
                float ratio = i * pointRatio;
 
                // Color
                Color color;
                if(colors.Length == 0)
                    color = Color.Lerp(Color.clear, Color.white, ratio);
                else if(colors.Length == 1)
                    color = Color.Lerp(Color.clear, colors[0], ratio);
                else if(colors.Length == 2)
                    color = Color.Lerp(colors[1], colors[0], ratio);
                else
                {
                    float colorRatio = colors.Length - 1 - ratio * (colors.Length-1);
                    if(colorRatio == colors.Length-1)
                        color = colors[colors.Length-1];
                    else
                    {
                        int min = (int) Mathf.Floor(colorRatio);
                        float lerp = colorRatio - min;
                        color = Color.Lerp(colors[min+0], colors[min+1], lerp);
                    }
                }
                meshColors[i * 2] = color;
                meshColors[(i * 2) + 1] = color;
 
                // Width
                float width;
                if(widths.Length == 0)
                    width = 1;
                else if(widths.Length == 1)
                    width = widths[0];
                else if(widths.Length == 2)
                    width = Mathf.Lerp(widths[1], widths[0], ratio);
                else
                {
                    float widthRatio = widths.Length - 1 - ratio * (widths.Length-1);
                    if(widthRatio == widths.Length-1)
                        width = widths[widths.Length-1];
                    else
                    {
                        int min = (int) Mathf.Floor(widthRatio);
                        float lerp = widthRatio - min;
                        width = Mathf.Lerp(widths[min+0], widths[min+1], lerp);
                    }
                }
 
                // Vertices
                if(faceCamera)
                {
                    Vector3 from = i == displayCnt-1 ?  points[i-1]   : point;
                    Vector3 to = i == displayCnt-1 ?    point    : points[i+1];
                    Vector3 pointDir = to - from;
                    Vector3 vectorToCamera = cameraPos - point;
                    Vector3 perpendicular = Vector3.Cross(pointDir, vectorToCamera).normalized;
                    vertices[i * 2 + 0] = point + perpendicular * width * 0.5f;
                    vertices[i * 2 + 1] = point - perpendicular * width * 0.5f;
                }
                else
                {
                    vertices[i * 2 + 0] = point + pointsUp[i] * width * 0.5f;
                    vertices[i * 2 + 1] = point - pointsUp[i] * width * 0.5f;
                }
 
                // UVs
                uvs[i * 2 + 0] = new Vector2(ratio , 0);
                uvs[i * 2 + 1] = new Vector2(ratio, 1);
 
                if(i > 0)
                {
                    // Triangles
                    int triIndex = (i - 1) * 6;
                    int vertIndex = i * 2;
                    triangles[triIndex+0] = vertIndex - 2;
                    triangles[triIndex+1] = vertIndex - 1;
                    triangles[triIndex+2] = vertIndex - 0;
 
                    triangles[triIndex+3] = vertIndex + 0;
                    triangles[triIndex+4] = vertIndex - 1;
                    triangles[triIndex+5] = vertIndex + 1;
                }
            }
            trail.transform.position = Cache.vector3Zero;
            trail.transform.rotation = Cache.quaternionIdentity;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.colors = meshColors;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        //}
        //catch(System.Exception e)
        //{
        //    print(e);
        //}
    }
}