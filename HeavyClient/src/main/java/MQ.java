import org.apache.activemq.ActiveMQConnectionFactory;

import javax.jms.*;

public class MQ {

    public static String callMQ() {
        try{
            ConnectionFactory connectionFactory = new ActiveMQConnectionFactory("tcp://localhost:61616");
            Connection connection = connectionFactory.createConnection();
            connection.start();
            Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);
            Destination destination = session.createQueue("queue");
            MessageConsumer consumer = session.createConsumer(destination);
            Message message = consumer.receive();
            javax.jms.TextMessage textMessage = (javax.jms.TextMessage) message;
            /**Print the message which contains our itinerary*/
            consumer.close();
            session.close();
            connection.close();
            return textMessage.getText();
        } catch (JMSException e) {
            throw new RuntimeException(e);
        }
    }
}
