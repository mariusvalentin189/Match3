using UnityEngine;

public class TileObject : MonoBehaviour
{
    public int id;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void PlayMoveAnimation(int direction)
    {
        // 0 - left, 1- right, 2-up, 3- down
        switch (direction)
        {
            case 0:
                anim.SetTrigger("left_move");
                break;

            case 1:
                anim.SetTrigger("right_move");
                break;

            case 2:
                anim.SetTrigger("up_move");
                break;

            case 3:
                anim.SetTrigger("down_move");
                break;

            default:
                break;
        }
    }
    public void ResetAnimationState()
    {
        anim.SetTrigger("Default");
    }
}
