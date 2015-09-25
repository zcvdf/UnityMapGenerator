using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour {
	int[,] map;
	System.Random pRandGen;
	
	public bool useRandomSeed = false;
	public string seed = "";
	public int chunckLength;
	public int maxAltitude;
	public int minAltitude;
	
	public int[] firstPoints;

	void Start(){
		generateMap();
		drawMapInCube();
	}

	void generateMap(){
		if (checkLenghtSize ()) {
			this.map = new int[this.chunckLength, this.chunckLength];
			startGeneration ();
			for (int i = 0; i < 5; i++) {
				smoothMap();
			}
		} else {
			Debug.LogError("Incorrect Chunk Length, must be 2^n+1");
		}
	}
	
	void startGeneration(){
		createDiamondSquareMap ();
	}
	
	bool checkLenghtSize(){
		if ((this.chunckLength - 1) % 2 == 0) {
			if(this.useRandomSeed){
				this.seed = System.DateTime.Now.ToString();
			}
			pRandGen = new System.Random(this.seed.GetHashCode());
			return true;
		}
		return false;
	}
	
	void createDiamondSquareMap(){
		if (firstPoints.IsFixedSize) {
			if(firstPoints.Length != 4){
				firstPoints = new int[4];
				firstPoints[0] = pRandGen.Next(minAltitude,maxAltitude);
				firstPoints[1] = pRandGen.Next(minAltitude,maxAltitude);
				firstPoints[2] = pRandGen.Next(minAltitude,maxAltitude);
				firstPoints[3] = pRandGen.Next(minAltitude,maxAltitude);
			} else {
				map[0,0] = firstPoints[0];
				map[0,chunckLength-1] = firstPoints[1];
				map[chunckLength-1,0] = firstPoints[2];
				map[chunckLength-1,chunckLength-1] = firstPoints[3];

			}
		}
		float scale = 1/this.chunckLength;
		int stepSize = this.chunckLength - 1;
		int a,b,c,d,e,f,g,h,i,x,y;
		while (stepSize >= 2) {
			int halfStep  = stepSize / 2;
			/*DIAMOND STEP
			 * a   b *
			 *   e   *
			 * c   d *
			 ********/
			/*x = 0;
			while(x < stepSize){
				y 
				while(y < stepSize){

				}
			}*/
			for(x = 0 ; x < this.chunckLength - 1 ; x += stepSize){
				for(y = 0 ; y < this.chunckLength - 1 ; y += stepSize){
					a = map[x,y];
					b = map[x+stepSize,y];
					c = map[x,y+stepSize];
					d = map[x+stepSize,y+stepSize];
					e = (a+b+c+d)/4 + scaling(stepSize,scale);
					map[x+halfStep,y+halfStep] = e;
				}
			}
			/*SQUARE STEP
			 * a g b *
			 * f e h *
			 * c i d *
			 ********/
			for(x = 0 ; x < this.chunckLength - 1 ; x += stepSize){
				for(y = 0 ; y < this.chunckLength - 1 ; y += stepSize){
					a = map[x,y];
					b = map[x+stepSize,y];
					c = map[x,y+stepSize];
					d = map[x+stepSize,y+stepSize];
					e = map[x+halfStep,y+halfStep];

					f = (a+e+c)/3 + scaling(stepSize,scale);
					map[x,y+halfStep] = f;
					g = (a+e+b)/3 + scaling(stepSize,scale);
					map[x+halfStep,y] = g;
					h = (e+b+d)/3 + scaling(stepSize,scale);
					map[x+halfStep+halfStep,y+halfStep] = h;
					i = (c+e+d)/3 + scaling(stepSize,scale);
					map[x+halfStep,y+halfStep+halfStep] = i;
				}
			}

			stepSize /= 2;

			scale *= 1.6f;
	    }
	}

	int scaling(int a, float b){
		return (pRandGen.Next(0,1)*2-1)*a*(int)b; // The Notch Way
	}

	void smoothMap(){
		for(int x = 0;  x < this.chunckLength; x++){
			for(int y = 0;  y < this.chunckLength; y++){
				map[x,y] = getNeighbourAverage(x,y);
			}
		}
	}
	
	int getNeighbourAverage(int gridX, int gridY){
		int average = 0;
		int nbrCase = 0;
		for (int localX = gridX - 1; localX <= gridX + 1; localX++) {
			for (int localY = gridY - 1; localY <= gridY + 1; localY++) {
				if(localX >= 0 && localX < this.chunckLength 
				   && localY >= 0 && localY < this.chunckLength
				   && localX != gridX && localY != gridY ){
					average += map[localX,localY];
					nbrCase++;
				}
			}
		}
		return (int)(average / nbrCase);
	}

	void drawMapInCube(){
		if(map != null){
			for(int x = 0; x < this.chunckLength; x++){
				for(int y = 0; y < this.chunckLength; y++){
					Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube),new Vector3(-this.chunckLength/2 + x + .5f, map[x,y], -this.chunckLength/2 + y + .5f),Quaternion.identity);
				}
			}
		}
	}
}
