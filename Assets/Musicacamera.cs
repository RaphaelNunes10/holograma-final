/*
 * Copyright 2020, 2019 Scott Hwang. All Rights Reserved.
 * This code was originally modified from example code 
 * in unity-sdk-4.0.0. This continueds to be licensed 
 * under the Apache License, Version 2.0 as noted below.
 *   
 */

/**
* Copyright 2018 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

#pragma warning disable 0649
using UnityEngine;
using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.TextToSpeech.V1;

using UnityEngine;
using UnityEngine.UI;

//celso abaixo
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine.Audio;


public class Musicacamera : MonoBehaviour
{

 

    private KeywordRecognizer KeywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    float speed = 50.0f;


    private AudioSource soundFix;
    
    //celso acima
    //TextToSpeech textspeec = new TextToSpeech();


    private void Start()
    {
        

        soundFix = GetComponent<AudioSource>();
        // Since coroutines can't return values I use the onValueChanged listener
        // to trigger an action after waiting for an input to arrive.
        // I originally used enums or flags to keep track if a process such
        // as obtaining a chat response from IBM Assistant was still being processed
        // or was finished processing but this was cumbersome.
    
        

        actions.Add("Tocar musica", tocamusica);
        
        


       
        KeywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        KeywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        KeywordRecognizer.Start();
    }

//


////celso acima
    private void Update()
    {

        if (Input.GetKeyDown("e"))
        {
            tocamusica();
            
        }

       


    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech){
        Debug.Log(speech.text);

        actions[speech.text].Invoke();

    }

    private void tocamusica(){
       soundFix.Play();
       TextToSpeech.ControlaDanca.Play("AnimacaoDanca.DancaBreak", -1, 0f);

    }
}


 

  