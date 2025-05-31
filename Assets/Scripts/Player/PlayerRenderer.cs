using UnityEngine;

public class PlayerRenderer : MonoBehaviour
{
    private Player player;
    private SpriteRenderer spriteRenderer;

    private Sprite[] allSprites;

    private Sprite[] walkDown;
    private Sprite[] walkLeft;
    private Sprite[] walkRight;
    private Sprite[] walkUp;

    private float frameTimer = 0f;
    private int currentFrame = 0;
    private readonly float frameRate = 0.1f;

    private Vector2 lastDir = Vector2.right; // 默认朝右

    private void Awake()
    {
        player = GetComponent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 加载 Resources 中的精灵图集
        allSprites = Resources.LoadAll<Sprite>("Sprites/Characters/Player");

        if (allSprites.Length < 24)
        {
            Debug.LogError("Player sprite sheet 切分不正确！");
            return;
        }

        // 按方向提取动画帧
        walkDown  = new Sprite[6];
        walkLeft  = new Sprite[6];
        walkRight = new Sprite[6];
        walkUp    = new Sprite[6];

        for (int i = 0; i < 6; i++)
        {
            walkDown[i]  = allSprites[i];       // 0~5
            walkLeft[i]  = allSprites[i + 6];   // 6~11
            walkRight[i] = allSprites[i + 12];  // 12~17
            walkUp[i]    = allSprites[i + 18];  // 18~23
        }
    }

    private void Update()
    {
        if (player == null) return;

        Vector2 input = player.InputDir;

        if (input != Vector2.zero)
        {
            lastDir = input.normalized;
        }

        UpdateAnimation(input);
    }

    public void UpdateAnimation(Vector2 input)
    {
        Sprite[] currentAnim = walkRight;

        if (input == Vector2.zero)
        {
            // 静止时选择每组的第一帧
            if (Mathf.Abs(lastDir.y) > Mathf.Abs(lastDir.x))
            {
                spriteRenderer.sprite = lastDir.y > 0 ? walkUp[0] : walkDown[0];
            }
            else
            {
                spriteRenderer.sprite = lastDir.x > 0 ? walkRight[0] : walkLeft[0];
            }

            return;
        }

        // 有输入时播放动画
        if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
        {
            currentAnim = input.y > 0 ? walkUp : walkDown;
        }
        else
        {
            currentAnim = input.x > 0 ? walkRight : walkLeft;
        }

        // 播放帧动画
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % currentAnim.Length;
            spriteRenderer.sprite = currentAnim[currentFrame];
        }
    }
}
