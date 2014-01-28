#ifndef __BIND_SSKYEFRAME__
#define __BIND_SSKYEFRAME__


#include "package_SpriteStudio.h"

class Bind_SsKeyValue
{
private:
	int			m_type;
	SsValue*	m_value;

	//�\���̂ɕϊ����Ȃ��Ă͂����Ȃ��̂�������

public:
	void	setValue( SsValue* v ) { m_value = v ; }
	void	setType( int t ){ m_type = t ;}
	int		getType(){ return m_type;}

	int		toInt(){ return m_value->get<int>();}
	float	toFloat(){ return m_value->get<float>();}
	bool	toBool(){ return m_value->get<bool>();}

//	SsColorAnime*	toColorAnime(); //������new����̂Ńo�C���h������
//	SsVertexAnime*	toVertexAnime(); //������new����̂Ńo�C���h������

};


class Bind_SsKeyframe : public myPyBinder<SsKeyframe>
{
private:
	Bind_SsKeyValue	m_tempValue;
	SsAttributeKind::_enum	m_attributeKind;

	SsColorAnime	m_tempcolor;
	SsVertexAnime	m_tempvertex;

public:

	Bind_SsKeyframe(){}
	Bind_SsKeyframe(SsKeyframe* a , SsAttributeKind::_enum attr ){bind(a);m_attributeKind = attr;}
	virtual ~Bind_SsKeyframe(){}

	int	time(){ return bind_inst->time; }
	int InterpolationType(){ return bind_inst->ipType; }
	SsCurve&	getCurveParam(){ return bind_inst->curve; }

	Bind_SsKeyValue&	getValue(){ 
		m_tempValue.setType( m_attributeKind ); // �^�ϊ��Ɏg�p
		m_tempValue.setValue(&bind_inst->value);
		return m_tempValue; }


	SsColorAnime&	getColorAnime(); 
	SsVertexAnime&	getVertexAnime(); 



};


#endif