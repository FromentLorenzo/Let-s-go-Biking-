import com.soap.ws.client.generated.GetRoute;
import com.soap.ws.client.generated.Service1;

public class Main {

    public static void main(String[] args) {

        Service1 service1 = new Service1();
        GetRoute getRoute = new GetRoute();
        String route = service1.getBasicHttpBindingIService1().getRoute("rue paul bert, lyon", "rue des rancy, lyon");
        System.out.println(route);

        // Place du Général-de-Gaulle, Rouen
        // Avenue Martyrs de la Résistance, Rouen
    }
}