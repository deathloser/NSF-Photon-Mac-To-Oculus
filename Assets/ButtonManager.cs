using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ButtonManager : MonoBehaviour
{
    private class gui_button
    {
        public int id { get; set; }
        public string script { get; set; }
        public string filename { get; set; }
        public string display_text { get; set; }
    }

    PhotonView photonView;

    public GameObject canvas;
    public GameObject button_prefab;
    public GameObject character;
    public TextAsset audio_csv;
    public string keep;
    Dictionary<string, int> field_map = new Dictionary<string, int>();
    Dictionary<string, string> anim_map = new Dictionary<string, string>();
    Dictionary<string, string> emote_map = new Dictionary<string, string>();
    // Start is called before the first frame update
    void Start()
    {
        // load csv info
        List<gui_button> buttons = load_csv(keep.ToLower());
        canvas = GameObject.Find("Canvas");
        character = GameObject.Find("PatientPrefab(Clone)");
        create_buttons(buttons);
        photonView = GetComponent<PhotonView>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // if (character) {
        //     Debug.Log("it is found");
        // } else {
        //     Debug.Log("not found");
        // }
    }

    [PunRPC]
    void ChatMessage(String audioName)
    {
        character = GameObject.Find("PatientPrefab(Clone)");
        Debug.Log(string.Format("ChatMessage: " + audioName));
        Debug.Log("playing clip");
        AudioSource track1 = character.GetComponent<AudioSource>();
        track1.clip = Resources.Load<AudioClip>(audioName);
        track1.Play();
    }

    private List<gui_button> load_csv(string keep)
    {
        List<gui_button> buttons = new List<gui_button>();
        string[] data = audio_csv.text.Split('\n');
        foreach (string row in data)
        {
            if (field_map.Count == 0) //header row
            {
                string[] field_names = row.Split(',');
                for (int i = 0; i < field_names.Length; i++)
                    field_map[field_names[i].ToLower()] = i;
            }
            else //data row
            {
                string[] cur_fields = row.Split(',');
                try{
                if (cur_fields[field_map["id"]].Length > 0) 
                {
                    if (keep.Length > 0)
                        if (cur_fields[field_map[keep]].Trim() != "1")
                            continue;
                    gui_button cur_button = new gui_button();
                    cur_button.script = cur_fields[field_map["script"]].Trim();
                    cur_button.filename = cur_fields[field_map["filename"]].Trim();
                    cur_button.id = int.Parse(cur_fields[field_map["id"]].Trim());
                    string button_text = cur_fields[field_map["button text"]].Trim();
                    cur_button.display_text = (button_text.Length > 0 ? button_text : cur_button.script);
                    buttons.Add(cur_button);
                    addAnimation(cur_fields);
                    addEmote(cur_fields);
                }
                }
                catch{
                    //Debug.Log(cur_fields[field_map["id"]].Length);
                }
            }
        }
        return buttons;
    }

     private void create_buttons(List<gui_button> buttons)
        {
        float horizontalInput = -210;
        float verticalInput = 160; 
        int i = 1;
        foreach (gui_button g in buttons)
        {
            GameObject cur_ = Instantiate(button_prefab, canvas.transform);
            UnityEngine.UI.Button cur_button = cur_.GetComponent<Button>();
            cur_button.name = g.script;
            cur_button.transform.localPosition = new Vector3(horizontalInput, verticalInput, 0);
            verticalInput = verticalInput - 27;
            if (i == 14 || i == 28 || i == 42){
                horizontalInput = horizontalInput + 129;
                verticalInput = 187;
            }
            i = i + 1;
            cur_button.GetComponentInChildren<TextMeshProUGUI>().text = g.display_text;
            String[] direc = g.filename.Split(".");
            //refer to characters audio source
            AudioSource track = cur_button.GetComponent<AudioSource>();
            cur_button.onClick.AddListener(delegate{TaskOnClick(direc[0]);});
        } 
        }

        void TaskOnClick(String audioName)
        { 
            //this is grabbing the audiosource from the empty game object and playing the audio that connects to salsa
            //grab the characters audio source and load the audio clip from resources
            // Debug.Log("playing clip");
            // AudioSource track1 = character.GetComponent<AudioSource>();
            // track1.clip = Resources.Load<AudioClip>(audioName);
            // track1.Play();
            // PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("ChatMessage", RpcTarget.All, audioName);

            //if animation or emote is present in the dictionary for this audio clip then it will call it
            // if (anim_map[audioName+".wav"] != ""){
            //     playAnimation(audioName);
            // }
            // if (emote_map[audioName+".wav"] != ""){
            //     playEmote(audioName);
            // }
        }
        // adds animation or emote to the dictionary if it is listed in the csv file
        void addAnimation(String[] cur_fields){
            anim_map.Add(cur_fields[field_map["filename"]], cur_fields[field_map["animation"]]);
        }

        void addEmote(String[] cur_fields){
            emote_map.Add(cur_fields[field_map["filename"]], cur_fields[field_map["emote"]]);
        }
        //plays the animation or emote if called
        void playAnimation(String x){
            // character.GetComponent<Animator>().CrossFade(anim_map[x+".wav"], 0.04f);
        }

        // void playEmote(String x){
        //     Emoter emote = character.GetComponent<Emoter>();
        //     emote.ManualEmote(emote_map[x+".wav"], ExpressionComponent.ExpressionHandler.RoundTrip, 1.5f);
        // }
}
