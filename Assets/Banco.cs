using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;



public class Banco : MonoBehaviour
{



   

   public string[] produtosBD = new string[] {"Macaquito"} ;

    private void Start(){
        Requ();
    }


    private void Update(){
        
    }


    public void Requ(){

            StartCoroutine(Perform());
    }

[Serializable]

 public class Manipula{


        public int id { get; set; }
        public string nome { get; set; }
         public Produto Produto { get; set; }    
        public Setor Setor { get; set; }


    }
    
[Serializable]

 public class Produto{
        public int id { get; set; }
        public string nome { get; set; }
        public string marca { get; set; }
        public string descricao { get; set; }
        public double preco { get; set; }
    }


[Serializable]

 public class Setor{
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string setor { get; set; }
        
    }





    IEnumerator Perform(){
       
        UnityWebRequest req = UnityWebRequest.Get("https://smi-2020.herokuapp.com/holograma");
        yield return req.SendWebRequest();

        var results = req.downloadHandler.text;      

        var obj = JsonConvert.DeserializeObject<List<Manipula>>(results);
    
        for(int i = 0; i<obj.Count; i++){
           
           // Debug.Log(obj[i].Produto.nome);
        //    produtosBD = new List<string>(produtosBD) { valorCompara[1].TrimStart('"').TrimEnd('"') }.ToArray();
            

        }

        



    }


}