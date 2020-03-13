using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MovementSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PlayerComponent playerComponent, ref Translation translation) =>
            {
                translation.Value = GameManager.GetInstance().NavMeshAgent.transform.position;
            });
    }
}
