import MyWebSocket from "./socket.js";

const serverPort = 5191

const messageList = getRequiredElementById('message-list')
const chatForm = getRequiredElementById('chat-form')
const connectForm = getRequiredElementById('connect-form')

const socket = new MyWebSocket(serverPort, appendMessage)

connectForm.addEventListener('submit', e => {
    e.preventDefault()

    const formData = new FormData(connectForm)
    const username = formData.get('username')

    socket.connect(username)
})

chatForm.addEventListener('submit', e => {
    e.preventDefault()

    const formData = new FormData(chatForm)
    const message = formData.get('chat-text')

    socket.send(message)
})

function appendMessage(message) {
    const newItem = document.createElement('li')
    const content = document.createTextNode(message)

    newItem.appendChild(content)

    messageList.appendChild(newItem)
}

function getRequiredElementById(id) {
    const element = document.getElementById(id)
    if (element == null) {
        throw new Error(`Element with ID '${id}' does not exist!`)
    }

    return element
}