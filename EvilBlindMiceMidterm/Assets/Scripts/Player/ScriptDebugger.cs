using UnityEngine;
using TMPro;
using System.Collections;

public class ScriptDebugger : MonoBehaviour
{
    [SerializeField] MonoBehaviour debugScript;
    [SerializeField] TMP_Text[] textsArray; // should have 8 references. No more, no less.

    IDebug debugInterface;

    DebugPacket debugPacket;

    bool cannotDebug = false;

    private void Start()
    {
        if (debugScript == null)
            cannotDebug = true;
        else
            debugInterface = (debugScript as IDebug);

        if (debugInterface == null) cannotDebug = true;
    }

    private void Update()
    {
        if (cannotDebug)
        {
            textsArray[0].text = "Selected script does not inherit IDebug";
            return;
        }

        StartCoroutine(DisplayDebugInfo());
    }

    private IEnumerator DisplayDebugInfo()
    {
        yield return new WaitForEndOfFrame();

        debugPacket = debugInterface.GetDebugPacket();

        textsArray[0].text = debugPacket.debug1;
        textsArray[1].text = debugPacket.debug2;
        textsArray[2].text = debugPacket.debug3;
        textsArray[3].text = debugPacket.debug4;
        textsArray[4].text = debugPacket.debug5;
        textsArray[5].text = debugPacket.debug6;
        textsArray[6].text = debugPacket.debug7;
        textsArray[7].text = debugPacket.debug8;
    }
}

public struct DebugPacket
{
    public string debug1;
    public string debug2;
    public string debug3;
    public string debug4;
    public string debug5;
    public string debug6;
    public string debug7;
    public string debug8;

    public DebugPacket(string _debug1 = "", string _debug2 = "", string _debug3 = "", string _debug4 = "", string _debug5 = "", string _debug6 = "", string _debug7 = "", string _debug8 = "")
    {
        debug1 = _debug1;
        debug2 = _debug2;
        debug3 = _debug3;
        debug4 = _debug4;
        debug5 = _debug5;
        debug6 = _debug6;
        debug7 = _debug7;
        debug8 = _debug8;
    }
}
