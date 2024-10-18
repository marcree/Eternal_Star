using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;
    //locationToLoadҪ���صĳ�����posToGo���Ŀ�ĵ����꣬fadeScreen�Ƿ��뽥��
    public void RaiseLoadRequestEvent(GameSceneSO locationToLoad,Vector3 posToGo,bool fadeScreen)
    {
        LoadRequestEvent ?.Invoke(locationToLoad, posToGo, fadeScreen);
    }
}
