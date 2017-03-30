#pragma strict

//Variables
//Esta variable es para poner una textura en 2D, en este caso es un logo, el cual se dibujara en la interfaz.

var frelook : FreeLookFromUnifyWiki;
//Esta variable es para la luz.
var luz : GameObject;
var luz2 : GameObject;
var luz3 : GameObject;
var luz4 : GameObject;
function Update(){
//Esta variable es para darle un estilo diferente al botón.
if(Input.GetKeyDown(KeyCode.Escape)){
frelook.enabled=!frelook.enabled;
}
}
//Aquí declaramos una función, en el caso de que no la pusieras no se te mostraría en pantalla nada de la interfaz.
function OnGUI(){
if(GUI.Button(Rect (10,10,128,64),"Light1")){
//----------------------------------------------------
// Aquí acabamos de hacer un botón en el cual pone "Hola", el cual está situado en x = 10, y = 10;
//Y tiene unas dimensiones de 128 x 64 pixeles.
//----------------------------------------------------
//Ahora haremos que cuando se presione se encienda y se apague una luz que previamente hemos colocado.
luz.active=!luz.active;

}


if(GUI.Button(Rect (150,10,128,64),"Light2")){
luz2.active=!luz2.active;
}


if(GUI.Button(Rect (300,10,128,64),"Light3")){
luz3.active=!luz3.active;
}

if(GUI.Button(Rect (450,10,128,64),"Light4")){
luz4.active=!luz4.active;
}


}
//Aquí cerramos la función.

