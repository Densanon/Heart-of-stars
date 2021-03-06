using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Resource : MonoBehaviour
{
    public static Action<ResourceData> OnClicked = delegate { };
    public static Action<ResourceData> OnUpdate = delegate { };

    Main main;

    [SerializeField]
    string myNamedResource;
    public ResourceData myResource;
    [SerializeField]
    string[] myNamedimediateDependencies;
    List<ResourceData> myImediateDependence;
    [SerializeField]
    string[] myNamedAllDependencies;
    ResourceData[] allMyDependence;
    [SerializeField]
    int[] dependencyAmounts;
    [SerializeField]
    bool[] dependenciesAble;

    public TMP_Text myText;

    public GameObject pivot;
    public GameObject buttonPrefab;
    public GameObject alphaButton;
    bool doneWithDependencyCheck = false;
    public bool Alpha = false;
    public bool panelButton = false;

    public void AssignResource(ResourceData data, bool alpha, Main theMain)
    {
        Alpha = alpha;
        myResource = data;
        main = theMain;
        myNamedResource = myResource.itemName;
        if(Alpha != true)
        {
            Debug.Log($"Resource: {data.displayName}: resource: {myResource}");
            Debug.Log($"Looking for text object? {myText}");
            if(myText != null)
            myText.text = myResource.displayName;
        }
        //Debug.Log($"My Resource is: {myResource.itemName}");

        myImediateDependence = new List<ResourceData>();

        List<string> tpDs = new List<string>();
        List<ResourceData> temp = new List<ResourceData>();
        List<int> otherTemp = new List<int>();

        //check for consumable dependencies
        string[] str = data.consumableRequirements.Split("-");
        if (data.consumableRequirements != "nothing=0")
        {
            foreach(string s in str)
            {
                string[] tAr = s.Split('=');
                //Debug.Log($"Looking for {tAr[0]}");
                temp.Add(main.ReturnData(tAr[0]));
                otherTemp.Add(int.Parse(tAr[1]));
                //Debug.Log($"My imediate resource dependency is: {temp[temp.Count - 1].itemName}");
                tpDs.Add(tAr[0]);
                Debug.Log($"Adding consumable {tAr[0]} to dependencies");
            }

        }

        //check for nonconsumable dependencies
        str = data.nonConsumableRequirements.Split("-");
        if(data.nonConsumableRequirements != "nothing")
        {
            if(str[0] != "nothing")
            {
                foreach (string s in str)
                {
                    string[] tAr = s.Split('=');
                    //Debug.Log($"Looking for {tAr[0]}");
                    temp.Add(main.ReturnData(tAr[0]));
                    otherTemp.Add(1);
                    //Debug.Log($"My imediate resource dependency is: {temp[temp.Count - 1].itemName}");
                    tpDs.Add(tAr[0]);
                    Debug.Log($"Adding nonconsumable {tAr[0]} to dependencies");
                }
            }
        }


        for(int i = 0; i < temp.Count; i++)
        {
            myImediateDependence.Add(temp[i]);
            Debug.Log($"These are: {temp[i].displayName}");
        }

        Debug.Log($"ImmediateDependencycount: {myImediateDependence.Count}");
        dependenciesAble = new bool[myImediateDependence.Count];
        dependencyAmounts = otherTemp.ToArray();
        myNamedimediateDependencies = tpDs.ToArray();


        if (Alpha)
        {
            GetAllDependencies(temp);
        }

        Debug.Log($"ImmediateDependencycount: second: {myImediateDependence.Count}");
        if(Alpha != true && transform.GetComponent<HoverAble>() != null)
        {
            HoverAble h = transform.GetComponent<HoverAble>();
            h.Assignment(this, main);
            if(panelButton)
                h.panelButton = true;

        }
    }

    void GetAllDependencies(List<ResourceData> dependencies)
    {
        //Debug.Log(dependencies.Count);
        List<ResourceData> extendedList = dependencies;
        List<ResourceData> temp = new List<ResourceData>();
        foreach(ResourceData r in dependencies)
        {
            temp.Add(r);
        }
        //Debug.Log($"Temp has {temp.Count} for all at start.");
        List<ResourceData> dump = new List<ResourceData>();
        string[] str;

        List<string> tempDependenceListNames = new List<string>();

        while (!doneWithDependencyCheck)
        {
            //Debug.Log($"extendedList has {extendedList.Count}");
            if (extendedList.Count == 0)
            {
                //Debug.Log("I found the end of the list.");
                doneWithDependencyCheck = true;
            }
            foreach (ResourceData dt in extendedList)
            {
                //Debug.Log("Going into the foreach loop.");
                str = dt.consumableRequirements.Split("-");
                //Debug.Log($"Looking at the new string array str {str[0]}");
                if (str[0] != "nothing=0")
                {
                    foreach (string s in str)
                    {
                        //Debug.Log($"Resource:GetAllDependencyCheck:Dependency: {s}");
                        string[] tAr = s.Split('=');
                        //Debug.Log($"Resource:GetAllDependencyCheck:Dependency: {tAr[0]}");
                        ResourceData TD = main.ReturnData(tAr[0]);
                        if (!temp.Contains(TD))
                        {
                            dump.Add(TD);
                            temp.Add(TD);
                            //Debug.Log($"In extensions: {dt.itemName} : dependency {TD.itemName}");
                            tempDependenceListNames.Add(TD.itemName);
                        }
                    }
                }

                str = dt.nonConsumableRequirements.Split("-");
                if (str[0] != "nothing")
                {
                    foreach (string s in str)
                    {
                        //Debug.Log($"Resource:GetAllDependencyCheck:Dependency: {s}");
                        string[] tAr = s.Split('=');
                        //Debug.Log($"Resource:GetAllDependencyCheck:Dependency: {tAr[0]}");
                        ResourceData TD = main.ReturnData(tAr[0]);
                        if (!temp.Contains(TD))
                        {
                            dump.Add(TD);
                            temp.Add(TD);
                            //Debug.Log($"In extensions: {dt.itemName} : dependency {TD.itemName}");
                            tempDependenceListNames.Add(TD.itemName);
                        }
                    }
                }
            }
            //Debug.Log($"Handing over {dump.Count} to extendedList.");
            extendedList.Clear();
            foreach(ResourceData d in dump)
            {
                extendedList.Add(d);
            }
            //Debug.Log($"extendedList now has {extendedList.Count}.");
            dump.Clear();
            //Debug.Log($"extendedList now has {extendedList.Count}.");
        }

        foreach(string st in myNamedimediateDependencies)
        {
            tempDependenceListNames.Add(st);
        }
        myNamedAllDependencies = tempDependenceListNames.ToArray();

        //Debug.Log($"Temp now has {temp.Count} at the end.");
        allMyDependence = temp.ToArray();
        SetupButtonLayout();
    }

    void SetupButtonLayout()
    {
        List<Resource> deps = new List<Resource>();
        //Debug.Log("Setting up dependence UI");

        if (Alpha)
        {
            Resource res = alphaButton.GetComponent<Resource>();

            res.AssignResource(myResource, false, main);
            deps.Add(res);
        }

        for(int i = allMyDependence.Length-1; i > -1; i--)
        {
            GameObject Obj = Instantiate(buttonPrefab, new Vector3(pivot.transform.position.x, pivot.transform.position.y + 100f, pivot.transform.position.z), Quaternion.identity, pivot.transform);
            Resource source = Obj.GetComponent<Resource>();
            source.AssignResource(allMyDependence[i], false, main);
            deps.Add(source);
            //Debug.Log($"Making {source.myResource.itemName} button.");
            if (i != 0)
            {
                pivot.transform.Rotate(0f, 0f, -45f);
                foreach(Resource r in deps)
                {
                    //r.Rotate();
                    r.ResetRotation();
                }
            }

        }
    }

    public void Rotate()
    {
        transform.Rotate(0f, 0f, 45f);
    }

    public void ResetRotation()
    {
        transform.rotation = Quaternion.identity;
    }

    public void ClickedButton()
    {
        if(myImediateDependence.Count != 0)
        {
            for(int i = 0; i < myImediateDependence.Count; i++)
            {
                if (myImediateDependence[i].currentAmount >= dependencyAmounts[i]) {
                    dependenciesAble[i] = true;
                }
                else{
                    dependenciesAble[i] = false;
                    return;
                }
            }

            if(myResource.groups == "tool" && myResource.currentAmount == myResource.atMostAmount){
                return;
            }

            for(int i = 0; i < myImediateDependence.Count; i++)
            {
                if(myImediateDependence[i].groups != "tool")
                {
                    Debug.Log($"Dependency: {myImediateDependence[i].displayName} has {myImediateDependence[i].currentAmount} and will be losing {dependencyAmounts[i]} " +
                        $"for a total of {myImediateDependence[i].currentAmount + dependencyAmounts[i] * -1}");
                    myImediateDependence[i].AdjustCurrentAmount(dependencyAmounts[i] * -1);
                    Debug.Log($"{myImediateDependence[i].displayName} now has {myImediateDependence[i].currentAmount}");
                    OnUpdate?.Invoke(myImediateDependence[i]);
                }
            }

            main.AddToQue(myResource, int.Parse(myResource.itemsToGain.Split("=")[1]));
            OnClicked?.Invoke(myResource);
            StartCoroutine(UpdateMyInformation());
        }
        else
        {
            if (myResource.groups == "tool" && myResource.currentAmount == myResource.atMostAmount)
            {
                return;
            }

            Debug.Log($"Clicking on basic resource: {myResource.displayName}");
            main.AddToQue(myResource, int.Parse(myResource.itemsToGain.Split("=")[1]));
            OnClicked?.Invoke(myResource);
            StartCoroutine(UpdateMyInformation());
        }
    }

    public ResourceData[] GetImediateDependencyNames()
    {
        //Debug.Log($"Resource: DependencyArray: {myImediateDependence}");
        return myImediateDependence.ToArray(); ;
    }

    public int[] GetDependencyAmounts()
    {
        return dependencyAmounts;
    }

    public void BecomeVisible()
    {
        myResource.AdjustVisibility(true);
        main.StartQueUpdate(myResource);

    }

    IEnumerator UpdateMyInformation()
    {
        yield return new WaitForSeconds(myResource.craftTime + 0.1f);
        OnUpdate?.Invoke(myResource);
    }
}
