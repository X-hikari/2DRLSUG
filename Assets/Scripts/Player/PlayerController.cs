using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;
    private SkillExecutor executor;
    private List<SkillData> allSkills;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();

        executor = new(player.gameObject);
        allSkills = SkillDataLoader.LoadSkillData();
        player.ownSkills[0] = allSkills[0];
        player.ownSkills[1] = allSkills[2];
        player.ownSkills[2] = allSkills[3];
        player.ownSkills[3] = allSkills[4];
    }

    private void Update()
    {
        HandleInput();
        player.buffManager.Update(Time.deltaTime);
    }

    public void HandleInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        player.moveInput = input;

        if (input != Vector2.zero)
        {
            player.lastMoveDir = input;
        }

        rb.velocity = input * player.stats.MoveSpeed;

        if (Input.GetKeyDown(KeyCode.Alpha1)) player.SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) player.SwitchWeapon(1);

        if (Input.GetKeyDown(KeyCode.Q)) executor.ExecuteSkill(player.ownSkills[0]);
        if (Input.GetKeyDown(KeyCode.E)) executor.ExecuteSkill(player.ownSkills[1]);
        if (Input.GetKeyDown(KeyCode.R)) executor.ExecuteSkill(player.ownSkills[2]);
        if (Input.GetKeyDown(KeyCode.T)) executor.ExecuteSkill(player.ownSkills[3]);
    }
}
