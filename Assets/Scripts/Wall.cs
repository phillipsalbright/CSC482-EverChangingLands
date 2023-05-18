using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall
{
    
    Vector2Int location;

    int condition;

    public Wall(Vector2Int position)
    {
        location = position;

    }

    public void reduceCondition() {
        if(condition > 0){
            condition--;
        }
    }

    public void setCondition(int cond) {
        condition = cond;
    }

    public int getCondition() {
        return condition;
    }
}
