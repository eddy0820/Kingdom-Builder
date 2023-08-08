using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class EdgeObject : PlaceableObject
{
    [Header("EdgeObject")]
    [SerializeField] List<EdgeObjectParentHolder> parentHolders;

    Dictionary<EdgeObjectParentHolder, IHasEdges> iHasEdgesParentDictionary = new Dictionary<EdgeObjectParentHolder, IHasEdges>();

    protected override void OnAwake()
    {
        iHasEdgesParentDictionary = new Dictionary<EdgeObjectParentHolder, IHasEdges>();

        foreach(EdgeObjectParentHolder parentHolder in parentHolders)
        {
            iHasEdgesParentDictionary.Add(parentHolder, null);
        }
    }

    protected override PlaceableObjectTypes GetObjectType()
    {
        return PlaceableObjectTypes.EdgeObject;
    }

    public void SetIHasEdgesObject(EdgeObjectParentHolder edgeObjectParentHolder, IHasEdges iHasEdges)
    {
        iHasEdgesParentDictionary[edgeObjectParentHolder] = iHasEdges;
    }

    public bool NullifyParentsThatMatch(IHasEdges iHasEdgeObject)
    {
        HashSet<EdgeObjectParentHolder> parentsToNullify = new HashSet<EdgeObjectParentHolder>();

        foreach(KeyValuePair<EdgeObjectParentHolder, IHasEdges> iHasEdgesParentEntry in iHasEdgesParentDictionary)
        {
            if(iHasEdgesParentEntry.Value == iHasEdgeObject)
            {
                parentsToNullify.Add(iHasEdgesParentEntry.Key);
            }
        }

        foreach(EdgeObjectParentHolder parentToNullifyHolder in parentsToNullify)
        {
            iHasEdgesParentDictionary[parentToNullifyHolder] = null;
        }

        return iHasEdgesParentDictionary.Values.All(x => x == null);
    }

    public override void DestroySelf()
    {
        foreach(KeyValuePair<EdgeObjectParentHolder, IHasEdges> iHasEdgesParentEntry in iHasEdgesParentDictionary)
        {
            if(iHasEdgesParentEntry.Value != null)
            {
                iHasEdgesParentEntry.Value.NullifyChildrenThatMatch(this);
            }
        }

        Destroy(gameObject);
    }
}
