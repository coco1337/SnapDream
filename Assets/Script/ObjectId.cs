using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ObjectId : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private CInteractableObject interactable;

    public void SetId(int i) => id = i;
    public int GetId => id;
    public CInteractableObject GetInteractableObject => interactable;
    public void BindObjectId(CInteractableObject obj) => interactable = obj;
}
