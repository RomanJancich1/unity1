using UnityEngine;
using UnityEngine.Events;

public class GridSumController : MonoBehaviour
{
    [Header("Grid (row-major, length must be N*N)")]
    public SnapSocket[] sockets = new SnapSocket[16];
    [Min(2)] public int N = 4;

    [Header("Target")]
    public int targetSum = 32;       
    public bool checkDiagonals = false; 

    [Header("Limits (optional)")]
    public int maxPlacements = 0;    
    public float timeLimitSec = 0f;  

    [Header("UI (optional)")]
    public TMPro.TextMeshPro statusText;
    public Color okColor = Color.green;
    public Color waitColor = new Color(1f, 0.8f, 0.2f, 1f);
    public Color failColor = Color.red;

    [Header("Events")]
    public UnityEvent OnSolved;
    public UnityEvent OnFailed;

    int placements;
    float t0;
    bool solved;

    void Start()
    {
        t0 = Time.time;
        Show($"Place all cubes (rows/cols must sum to {targetSum})", waitColor);
    }

    void Update()
    {
        if (solved) return;

        if (timeLimitSec > 0f && Time.time - t0 > timeLimitSec)
        {
            Fail("Time up");
            return;
        }

        for (int i = 0; i < N * N; i++)
        {
            if (sockets[i].current == null)
            {
                Show("Place all cubes", waitColor);
                return;
            }
        }

        int[] v = new int[N * N];
        for (int i = 0; i < N * N; i++) v[i] = sockets[i].current.value;

        for (int r = 0; r < N; r++)
        {
            int sum = 0;
            for (int c = 0; c < N; c++) sum += v[r * N + c];
            if (sum != targetSum) { Show($"Row {r + 1} ≠ {targetSum}", failColor); return; }
        }
        for (int c = 0; c < N; c++)
        {
            int sum = 0;
            for (int r = 0; r < N; r++) sum += v[r * N + c];
            if (sum != targetSum) { Show($"Col {c + 1} ≠ {targetSum}", failColor); return; }
        }
        if (checkDiagonals)
        {
            int d1 = 0, d2 = 0;
            for (int i = 0; i < N; i++) { d1 += v[i * N + i]; d2 += v[i * N + (N - 1 - i)]; }
            if (d1 != targetSum || d2 != targetSum) { Show("Diagonal mismatch", failColor); return; }
        }

        solved = true;
        Show("Solved!", okColor);
        OnSolved?.Invoke();
    }

    public void CountPlacement()
    {
        if (maxPlacements <= 0) return;
        placements++;
        if (placements > maxPlacements) Fail("Move limit exceeded");
    }

    void Fail(string why)
    {
        Show(why, failColor);
        OnFailed?.Invoke();
    }

    void Show(string msg, Color c)
    {
        if (!statusText) return;
        statusText.text = msg;
        statusText.color = c;
    }
}
