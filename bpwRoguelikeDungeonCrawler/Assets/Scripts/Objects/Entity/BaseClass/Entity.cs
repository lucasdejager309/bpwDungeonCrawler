using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Entity : MonoBehaviour
{
    public LayerMask solidLayer;
    public bool entityIsSolid;

    void Start() {
        EntityManager.Instance.UpdatePos(this);
    }

    protected virtual IEnumerator Move(Vector2Int direction, int distance = 1, bool smoothMove = false, float moveTime = 0.2f, float waitBetweenMoves = 0) {
            EntityManager.Instance.UpdatePos(this); //ugly fix

            //get actual position to move to (incase target position is unreachable)
            Vector2Int newPos = EntityMovement.GetNewPosition(transform, direction, distance);

            //move to actual position
            if (EntityMovement.IsValidMovePos(newPos)) {
                if (!smoothMove) {
                    transform.position = new Vector3(newPos.x, newPos.y, 0);

                    EntityManager.Instance.UpdatePos(this);
                } else {
                    Vector3 startPos = transform.position;
                    Task t = new Task(EntityMovement.SmoothMove(transform, newPos, moveTime/distance, waitBetweenMoves));
                    t.Finished += delegate (bool manual) {
                        transform.position = new Vector3(newPos.x, newPos.y, 0);

                        EntityManager.Instance.UpdatePos(this);
                    };
                    yield return new WaitForSeconds(moveTime+waitBetweenMoves);
                }
            } else yield break;
    }

    public IEnumerator MoveToWards(GameObject target, int tilesPerMove, float moveSpeed) {
        Vector2Int targetPos = target.GetComponent<Entity>().GetPos();
        
        List<PathNode> path = Pathfinding.FindPath(new PathNode(GetPos()), new PathNode(targetPos), EntityManager.Instance.validPositions);
        if (path != null) {
            GameManager.Instance.DrawPath(path);

            int i = 0;
            bool doingMove = false;
            while(i < tilesPerMove && i < path.Count) {
                if (!doingMove) {
                    doingMove = true;
                    Vector2 dir = GetDirToNeighbour(path[i].pos);
                    Task t = new Task(Move(new Vector2Int((int)dir.x, (int)dir.y), 1, true, moveSpeed));
                    t.Finished += delegate {
                        i++;
                        doingMove = false;
                    };
                }

                yield return null;
            }
        } else yield return null;
    }

    //IM SURE THERE IS A BETTER WAY TO DO THIS HOLY SHIT
    private Vector2Int GetDirToNeighbour(Vector2Int pos) {
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) { continue;}

                if (GetPos()+new Vector2Int(x,y) == pos) {
                    return new Vector2Int(x,y);
                }
            }
        }
        return new Vector2Int(0,0);
    }

    public bool TileInSight(Vector2 posToCheck) {
       Vector2 pos = new Vector2(GetPos().x+0.5f, GetPos().y+0.5f);
       posToCheck = new Vector2(posToCheck.x+0.5f, posToCheck.y+0.5f);
       RaycastHit2D hit = Physics2D.Raycast(pos, (posToCheck-pos), (posToCheck-pos).magnitude, solidLayer);
       //Debug.DrawRay(pos, posToCheck-pos, Color.cyan, 1f);
       if (hit.collider != null) {
           if (hit.collider.tag == "solidTileMap") {
               return false;
           } else return true;
       } else return true;
    }

    public virtual IEnumerator DoAction() {
        yield break;
    }

    public virtual void DeleteEntity() {
        EntityManager.Instance.entityDict.Remove(this);
        GameObject.Destroy(gameObject);
    } 

    public Vector2Int GetPos() {
        // UpdatePos();
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }
}
