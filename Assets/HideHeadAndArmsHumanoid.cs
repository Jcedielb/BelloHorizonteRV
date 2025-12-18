using UnityEngine;
using System.Linq;

public class HideHeadAndArmsHumanoid : MonoBehaviour
{
    public bool hideHead = true;
    public bool hideArms = false;

    void Start()
    {
        var animator = GetComponent<Animator>();
        if (!animator || !animator.isHuman) return;

        var smrList = GetComponentsInChildren<SkinnedMeshRenderer>(true);

        if (hideHead)
        {
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            HideRendererBelow(head, smrList);
        }
        if (hideArms)
        {
            HideRendererBelow(animator.GetBoneTransform(HumanBodyBones.LeftShoulder), smrList);
            HideRendererBelow(animator.GetBoneTransform(HumanBodyBones.RightShoulder), smrList);
        }
    }

    void HideRendererBelow(Transform bone, SkinnedMeshRenderer[] all)
    {
        if (!bone) return;
        foreach (var smr in all)
        {
            var bones = smr.bones;
            if (bones.Any(b => b == bone || (b && bone.IsChildOf(b)))) smr.enabled = false;
        }
    }
}
