using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimPointer : MonoBehaviour
{   
    Vector2Int currentPos;

    public ThrowableItem itemToThrow;

    public Sprite validPosSprite;
    public Sprite invalidPosSprite;

    SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPos(Vector2Int newPos) {
        transform.position = new Vector3(newPos.x + 0.5f, newPos.y+0.5f, 0);
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
        SetPos(newPos);

        RangedAttack ranged = GameManager.Instance.player.GetComponent<RangedAttack>();
        if (!ranged.HasAimOnTarget(newPos)) {
            spriteRenderer.sprite = invalidPosSprite;
        } else {
            spriteRenderer.sprite = validPosSprite;
        }
    }
}
