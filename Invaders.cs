using UnityEngine;

public class Invaders : MonoBehaviour
{
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[5];
    public AnimationCurve speed = new AnimationCurve();
    private Vector3 direction = Vector3.right;
    private Vector3 initialPosition;

    [Header("Grid")]
    public int rows = 5;
    public int columns = 11;

    [Header("Missiles")]
    public Projectile missilePrefab;
    public float missileSpawnRate = 1f;

    private void Awake()
    {
        initialPosition = transform.position;

        CreateInvaderGrid();
    }

    private void CreateInvaderGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            float width = 2f * (columns - 1);
            float height = 2f * (rows - 1);

            Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (2f * i) + centerOffset.y, 0f);

            for (int j = 0; j < columns; j++)
            {
                Invader invader = Instantiate(prefabs[i], transform);   // Create a new invade
                Vector3 position = rowPosition;  // Calculate and set the position
                position.x += 2f * j;
                invader.transform.localPosition = position; 
            }
        }
    }
    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
    }

    private void MissileAttack()
    {
        int amountAlive = GetAliveCount();
        if (amountAlive == 0) {
            return;
        }
        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy) { // Any invaders that are killed cannot shoot missiles
                continue;
            }
            if (Random.value < (1f / amountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }

    private void Update()
    {
        int totalCount = rows * columns;
        int amountAlive = GetAliveCount();
        int amountKilled = totalCount - amountAlive;
        float percentKilled = amountKilled / (float)totalCount;  // Evaluate the speed of the invaders
        float speed = this.speed.Evaluate(percentKilled);
        transform.position += speed * Time.deltaTime * direction;
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero); // reach the edge of the screen
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right); //reach the edge of the screen
        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy) {
                continue;
            }
            if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1f))
            {
                AdvanceRow();
                break;
            }
            else if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1f))
            {
                AdvanceRow();
                break;
            }
        }
    }

    private void AdvanceRow()
    {
        direction = new Vector3(-direction.x, 0f, 0f); // Flip the direction of invaders
        Vector3 position = transform.position;
        position.y -= 1f;
        transform.position = position;
    }
    public void ResetInvaders()
    {
        direction = Vector3.right;
        transform.position = initialPosition;
        foreach (Transform invader in transform) {
            invader.gameObject.SetActive(true);
        }
    }
    public int GetAliveCount()
    {
        int count = 0;
        foreach (Transform invader in transform)
        {
            if (invader.gameObject.activeSelf) {
                count++;
            }
        }
        return count;
    }
}
