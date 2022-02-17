using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternMatcher {

    // there is 9 points, because the graph is fixed 
    //1 2 3 
    //4 5 6
    //7 8 9
    private List<(int a, int b)> m_input;
    
    public PatternMatcher(List<(int a, int b)> inputs) {
        this.m_input = new List<(int, int)>(inputs);
    }

    public bool IsStrictlySame(PatternMatcher rhs) {
        var this_input = new List<(int a, int b)>(this.m_input);
        // int count = 0;
        foreach (var element_rhs in rhs.m_input)
        {
            // count++;
            // foreach (var element_this in this.m_input){
            //     bool t = false;
            //     if (element_rhs.a == element_this.a && element_rhs.b == element_this.b){
            //         t = true;
            //         delete element_rhs;
            //         delete element_this;
            //     }
            //     else if(element_rhs.a == element_this.b && element_rhs.b == element_this.a){
            //         t = true;
            //         delete element_rhs;
            //         delete element_this;
            //     }
            //     if (t == true){
            //         break;
            //     }
            // }
            // if (t==false)return false;

            // if (rhs.m_input == null && this.m_input == null){
            //     return true;
            // }
            var swp = (a: element_rhs.b, b: element_rhs.a);
            if(!this_input.Remove(element_rhs)) {
                if(!this_input.Remove(swp)) {
                    return false;
                }
            }
        }
        return this_input.Count == 0;
        // if (rhs.m_input != null || this.m_input != null){
        //     return false;
        // }
    }

    static PatternMatcher mirror(PatternMatcher pat) {
        var new_list = new List<(int a, int b)>();
        foreach (var element in pat.m_input){
            (int a, int b) T;
            if((element.a == 1)||(element.a==4)||(element.a==7)) {
                T.a = element.a + 2;
            } else if((element.a == 3)||(element.a==6)||(element.a==9)) {
                T.a = element.a - 2;
            } else {
                T.a = element.a;
            }
            if((element.b == 1)||(element.b==4)||(element.b==7)) {
                T.b = element.b + 2;
            } else if((element.b == 3)||(element.b==6)||(element.b==9)) {
                T.b = element.b - 2;
            } else {
                T.b = element.b;
            }
            // T.b = element.b;
            new_list.Add(T);
        } 
        return new PatternMatcher(new_list);
    }

    static PatternMatcher rotate90deg(PatternMatcher pat) {
        var new_list = new List<(int a, int b)>();
        int[] mapping = {0, 7, 4, 1, 8, 5, 2, 9, 6, 3};
        // return new PatternMatcher(pat.m_input.Select(item => (mapping[item.a], mapping[item.b])).ToList());
        foreach (var element in pat.m_input){
            // (int a, int b) T;
            // if(pat.a == 1) {
            //     T.a = 7;
            // } else if(pat.a ==2) {
            //     T.a = element.a - 2;
            // } else {
            //     T.a = element.a;
            // }
            // if((pat.b == 1)||(pat.b==4)||(pat.b==7)) {
            //     T.b = element.b + 2;
            // } else if((pat.b == 3)||(pat.b==6)||(pat.b==9)) {
            //     T.b = element.b - 2;
            // } else {
            //     T.b = element.b;
            // }
            // T.b = element.b;
            // new_list.Insert(T); 
            new_list.Add((mapping[element.a], mapping[element.b]));
        } 
        return new PatternMatcher(new_list);
    }

    public bool IsSame(PatternMatcher rhs) {
        // this.m_input
        // rhs.m_input
        for(int i=0; i<4;i++){
            if (IsStrictlySame(rhs)){
                return true;
            } else if(IsStrictlySame(mirror(rhs))){
                return true;
            }
            rhs = rotate90deg(rhs);
        }
        return false;
    }
}
