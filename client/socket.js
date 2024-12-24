export default class MyWebSocket {
    constructor(serverPort, receive) {
        if (serverPort < 0 || serverPort > 65565)
            throw new RangeError();

        this.serverPort = serverPort;
        this.socket = new WebSocket(`ws://localhost:${serverPort}/ws`)
        this.connectionOpened = false;

        this.socket.addEventListener('open', e => {
            this.connectionOpened = true
        })
        
        this.socket.addEventListener('message', e => {
            receive(e.data)
        })
    
        this.socket.addEventListener('error', e => {
            console.log(e)
        })
    }

    send(message) {
        if (this.connectionOpened === false)
            return false
        
        this.socket.send(message)
        return true
    }

    connect(username) {
       if (this.connectionOpened === false)
        return false; 

        const initialPayload = JSON.stringify({
            username: username
        })

        this.socket.send(initialPayload)

        return true;
    }
}