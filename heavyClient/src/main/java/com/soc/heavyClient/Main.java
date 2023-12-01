package com.soc.heavyClient;

import com.soap.ws.client.generated.*;

public class Main {
    public static void main(String args[]) {
        System.out.println("hello world !");
        Service1 service1 = new Service1();
        System.out.println("1");
        IService1 iService1 = service1.getWSHttpBindingIService1();
        System.out.println("2");
        String res = iService1.getRoute("Rue du Boeuf, Lyon", "1 Rue de la Paix, Nantes");

        System.out.println(res);

    }


}
