#include<cmath>
#include<eigen3/Eigen/Core>
#include<eigen3/Eigen/Dense>
#include<iostream>

int main(){
    Eigen::Vector3f p(2.0f,1.0f,1.0f);
    Eigen::Matrix3f R, T;
    float ftheta=sqrt(2.0f)/2;
    R<<ftheta,-ftheta,0,
       ftheta,ftheta,0,
       0,0,1.0f;

    p=R*p;
    std::cout<<"after rotate \n";
    std::cout<<p<<std::endl;

    p=T*p;
    std::cout<<"after transformation \n";
    std::cout<<p<<std::endl;

    return 0;
}