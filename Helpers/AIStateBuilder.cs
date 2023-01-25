using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStateBuilder<T> where T : AIState
{
    public string Name;
    private AIRoot AIRoot;
    private CharacterAI CharacterAI;
    public GameObject StateGameObject { get; private set; }
    public AIState StateComp { get; private set; }

    public int StateIndex { get; private set; }


    public const string ValidEffectsTransformName = "ValidEffects";
    public const string InvalidEffectsTransformName = "InvalidEffects";

    private Dictionary<string, GameObject> ConditionGOs = new Dictionary<string, GameObject>();

    public AIStateBuilder(string name, CharacterAI characterAI)
    {
        Name = name;
        CharacterAI = characterAI;
        AIRoot = characterAI.AIStatesRoot;
        ConditionGOs = new Dictionary<string, GameObject>();
        StateGameObject = CreateStateGameObject();
    }

    /// <summary>
    /// Initalizes and adds the AIStateGameObject to the Target CharacterAI
    /// </summary>
    public void Apply(bool SetActive = true)
    {
        //add state to AI
        StateGameObject.transform.parent = AIRoot.transform;
        //set as last
        StateGameObject.transform.SetAsLastSibling();
        //get sibling index
        int sibIndex = StateGameObject.transform.GetSiblingIndex();
        StateGameObject.transform.name = $"{sibIndex + 1}_{Name}";
        StateIndex = sibIndex;


        StateComp = CreateAIState(StateGameObject, StateIndex);

        if (StateComp)
        {
            StateComp.Init(CharacterAI);
        }

        CharacterAI.m_aiStates = CharacterAI.m_aiStates.Append(StateComp).ToArray();




        if (SetActive)
        {
            CharacterAI.SwitchAiState(StateIndex);
        }
    }



    /// <summary>
    /// Create a new AIState GameObject and add the component.
    /// </summary>
    /// <param name="StateGameObject"></param>
    /// <param name="CharacterAI"></param>
    /// <param name="StateIndex"></param>
    /// <returns></returns>
    private AIState CreateAIState(GameObject StateGameObject, int StateIndex)
    {
        T newStateComp = StateGameObject.AddComponent<T>();
        newStateComp.m_stateID = StateIndex;
        //newStateComp.Init(CharacterAI);
        return newStateComp;
    }

    /// <summary>
    /// Adds a AICondition GameObject to the CurrentState, it returns the created AICondition component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="GameObjectName"></param>
    /// <returns></returns>
    public AICondition AddCondition<T>(string GameObjectName) where T : AICondition
    {
        if (StateGameObject == null)
        {
            //OutSouls.OutwardSouls.Log("AIStateBuilder AddCondition StateGameObject is null");
            return null;
        }


        GameObject newConditionGO = null;
        Transform validEffects = null;
        Transform invalidEffects = null;


        if (!ConditionGOs.ContainsKey(GameObjectName))
        {
            //OutSouls.OutwardSouls.Log($"AIStateBuilder AddCondition {GameObjectName} does not exist");
            newConditionGO = CreateConditionGameObject(StateGameObject.transform, GameObjectName);
            validEffects = GetValidEffectsTransform(newConditionGO);
            invalidEffects = GetInvalidEffectsTransform(newConditionGO);
            ConditionGOs.Add(GameObjectName, newConditionGO);
        }
        else
        {
            //OutSouls.OutwardSouls.Log($"AIStateBuilder AddCondition {GameObjectName} exists");
            newConditionGO = ConditionGOs[GameObjectName];
            validEffects = GetValidEffectsTransform(newConditionGO);
            invalidEffects = GetInvalidEffectsTransform(newConditionGO);
        }
    
        //OutSouls.OutwardSouls.Log($"AIStateBuilder AddCondition {GameObjectName} adding component");
        T newConditionComp = newConditionGO.AddComponent<T>();

        if (newConditionComp.m_conditions == null)
        {
            newConditionComp.m_conditions = new AICondition[]
            {
                newConditionComp
            };
        }
        else
        {
            newConditionComp.m_conditions.Append(newConditionComp).ToArray();
        }

        newConditionComp.ChildConditions = new AICondition[0];
        newConditionComp.SubCondition = false;
        newConditionComp.ValidEventOnCharacter = "";
        newConditionComp.ValidEventOnParent = "";
        newConditionComp.InvalidEventOnCharacter = "";
        newConditionComp.InvalidEventOnParent = "";
        newConditionComp.GroupValidEffectTrans = validEffects;
        newConditionComp.GroupInvalidEffectTrans = invalidEffects;
        return newConditionComp;
    }

    /// <summary>
    /// Adds an AIEffect ValidEffect for the AICondition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ParentCondition"></param>
    /// <returns></returns>
    public AIEffect AddValidConditionEffect<T>(AICondition ParentCondition) where T : AIEffect
    {
        //OutSouls.OutwardSouls.Log($"AIStateBuilder Adding valid condition {typeof(T)} to {ParentCondition.GroupValidEffectTrans}");
        if (ParentCondition && ParentCondition.GroupValidEffectTrans)
        {
            //GameObject newConditionEffectGO = new GameObject(EffectGOName);
            //newConditionEffectGO.transform.parent = ParentCondition.GroupValidEffectTrans;
            //newConditionEffectGO.transform.SetAsLastSibling();
            return ParentCondition.GroupValidEffectTrans.gameObject.AddComponent<T>();
        }
        else
        {
            OutSouls.OutwardSouls.Log("No AICOndition coponent found on" + ParentCondition);
        }

        return null;
    }



    private GameObject CreateStateGameObject()
    {
        //create GO
        GameObject StateGameObject = new GameObject(Name);
        return StateGameObject;
    }
    private GameObject CreateConditionGameObject(Transform StateRoot, string GameObjectName)
    {
        GameObject newConditionGO = new GameObject(GameObjectName);
        newConditionGO.transform.parent = StateRoot.transform;
        newConditionGO.transform.SetAsLastSibling();

        GameObject validEffects = new GameObject(ValidEffectsTransformName);
        validEffects.transform.parent = newConditionGO.transform;
        validEffects.transform.SetAsLastSibling();

        GameObject invalidEffects = new GameObject(InvalidEffectsTransformName);
        invalidEffects.transform.parent = newConditionGO.transform;
        invalidEffects.transform.SetAsLastSibling();
        return newConditionGO;
    }
    private Transform GetValidEffectsTransform(GameObject ConditionGameObject)
    {
        Transform validTransform = ConditionGameObject.transform.Find(ValidEffectsTransformName);

        if (validTransform == null)
        {
            GameObject validEffects = new GameObject(ValidEffectsTransformName);
            validEffects.transform.parent = ConditionGameObject.transform;
            validEffects.transform.SetAsLastSibling();

            validTransform = validEffects.transform;
        }

        return validTransform;
    }
    private Transform GetInvalidEffectsTransform(GameObject ConditionGameObject)
    {
        Transform invalidTransform = ConditionGameObject.transform.Find(InvalidEffectsTransformName);

        if (invalidTransform == null)
        {
            GameObject invalidEffects = new GameObject(InvalidEffectsTransformName);
            invalidEffects.transform.parent = ConditionGameObject.transform;
            invalidEffects.transform.SetAsLastSibling();
            invalidTransform = invalidEffects.transform;
        }

        return invalidTransform;
    }


    public static List<AICondition> GetAllConditionsForState<T>(CharacterAI characterAI) where T : AIState
    {
        if (HasState<T>(characterAI))
        {
            List<AICondition> conditions = new List<AICondition>();

            List<AIState> FoundState = GetStateByType<T>(characterAI);

            if (FoundState.Count > 0)
            {
                return FoundState[0].m_conditions.ToList();
            }
        }

        return null;
    }
    public static bool HasState<T>(CharacterAI CharacterAI) where T : AIState
    {
        return GetAllCurrentStates<T>(CharacterAI).Find(x => x.GetType() == typeof(T)) != null ? true : false;
    }
    public static List<AIState> GetAllCurrentStates<T>(CharacterAI characterAI) where T : AIState
    {
        List<AIState> aIStates = new List<AIState>();

        for (int i = 0; i < characterAI.AIStatesRoot.transform.childCount; i++)
        {
            AIState state = characterAI.AIStatesRoot.transform.GetChild(i).gameObject.GetComponent<AIState>();

            if (state != null)
            {
                aIStates.Add(state);
            }
        }

        return aIStates;
    }
    public static List<AIState> GetStateByType<T>(CharacterAI characterAI) where T : AIState
    {
        List<AIState> aIStates = new List<AIState>();

        foreach (var item in characterAI.AIStatesRoot.GetComponents<T>())
        {
            aIStates.Add(item);
        }

        return aIStates;
    }
}