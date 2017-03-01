
function Person(props) {

    return (
        <div className="person-card">
            <div className="cardcontent">
                {props.first} {props.last}
            </div>

        </div>

  );
}

class PersonList extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            //list: [{ id: 1, first: 'julio', last: 'm' }, { id: 2, first: 'julio', last: 'mx' }, { id: 3, first: 'julio', last: 'mx' }],
            list: [],

        }

    }
    componentDidMount() {

        $.ajax({
            url: "http://localhost:53300/Api/persons",
            type: "GET",
            crossDomain: true,
            dataType: "json",
            success: (response) => {
                //console.log("response");
                //console.log(response);
                this.setState({ list: response });
            },
            error: function (xhr, status) {
                alert("error");
            }
        });
        
    }
    render() {
        var content = this.state.list.map((current) => {
            return <Person key={current.id} first={current.first} last={current.last }></Person>
        });
        
        return (<div>{content}</div>);

    }

}
ReactDOM.render(
      <PersonList />,
      document.getElementById('jsxcontent')
    );