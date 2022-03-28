    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimPointer : MonoBehaviour
{   
    Vector2Int currentPos;

    public ThrowableItem itemToUse;

    public Sprite validPosSprite;
    public Sprite invalidPosSprite;

    SpriteRenderer spriteRenderer;

    public bool ignoreWalls;
    public int standardRange = 6;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void IgnoreWalls(bool state) {
        ignoreWalls = state;
    }

    public void SetPos(Vector2Int newPos) {
        transform.position = new Vector3(newPos.x, newPos.y, 0);
        currentPos = newPos;
    }

    public Vector2Int GetPos() {
        return currentPos; 
    }

    public void SetActive(bool state) {
        spriteRenderer.enabled = state;
    }

    public void UpdatePos(Vector2Int input) {
        Vector2Int newPos = GetPos() + input;
        int range;
        if (itemToUse != null) {
            range = itemToUse.throwRange;
        } else range = standardRange;
        if (Vector2.Distance(newPos, GameManager.Instance.player.GetComponent<Player>().GetPos()) < range) {
            SetPos(newPos);
        }
        

        RangedAttack ranged = GameManager.Instance.player.GetComponent<RangedAttack>();
        if (!ranged.HasAimOnTarget(newPos) && ignoreWalls == false) {
            spriteRenderer.sprite = invalidPosSprite;
        } else {
            spriteRenderer.sprite = validPosSprite;
        }
    }
}
