describe( "Add new buttons", function() {
	var table;

	dt.libs( {
		js:  [ 'jquery', 'datatables', 'buttons' ],
		css: [ 'datatables', 'buttons' ]
	} );

	dt.html( 'basic' );

	it( 'Create DataTable with no buttons', function () {
		table = $('#example').DataTable( {
			dom: "Bfrtip",
			buttons: []
		} );

		expect( table.buttons().count() ).toBe( 0 );
	});

	it( 'Add a new button', function () {
		table.button().add( null, {
			text: '1'
		} );
		expect( table.buttons().count() ).toBe( 1 );
	} );

	it( 'Insert at index 0 inserts before first button', function () {
		table.button().add( 0, {
			text: '2'
		} );
		expect( table.buttons().count() ).toBe( 2 );
		expect( $('a.dt-button:first').text() ).toBe( '2' );
	} );

	it( 'Insert at index 2 inserts after current last button', function () {
		table.button().add( 2, {
			text: '3'
		} );
		expect( table.buttons().count() ).toBe( 3 );
		expect( $('a.dt-button:last').text() ).toBe( '3' );
	} );

	it( 'Insert button with a string based button', function () {
		table.button().add( 0, 'pageLength' );
		expect( table.buttons().count() ).toBe( 8 );
		expect( $('a.dt-button:first').text() ).toBe( 'Show 10 rows' );
	} );

	it( 'Insert function based button', function () {
		table.button().add( 0, function () {
			return { text: 'Function' };
		} );
		expect( table.buttons().count() ).toBe( 9 );
		expect( $('a.dt-button:first').text() ).toBe( 'Function' );
	} );


	dt.html( 'basic' );

	it( 'Create DataTable with two button groups', function () {
		table = $('#example').DataTable( {
			dom: "Bfrtip",
			buttons: {
				buttons: [],
				name: 'first'
			}
		} );

		new $.fn.dataTable.Buttons( table, {
			name: 'second',
			buttons: []
		} );

		table.buttons( 'second', null ).container().appendTo( 'body' );

		expect( table.buttons( 'first', null ).count() ).toBe( 0 );
		expect( table.buttons( 'second', null ).count() ).toBe( 0 );
	});

	it( 'Add a button to the second group', function () {
		table.button( 'second', null ).add( null, {
			text: '1'
		} );

		expect( table.buttons( 'first', null ).count() ).toBe( 0 );
		expect( table.buttons( 'second', null ).count() ).toBe( 1 );
	});

	it( 'Add a button to the first group', function () {
		table.button( 'first', null ).add( null, {
			text: '2'
		} );

		expect( table.buttons( 'first', null ).count() ).toBe( 1 );
		expect( table.buttons( 'second', null ).count() ).toBe( 1 );

		expect( $('a.dt-button:first').text() ).toBe( '2' );
		expect( $('a.dt-button:last').text() ).toBe( '1' );
	});
});
